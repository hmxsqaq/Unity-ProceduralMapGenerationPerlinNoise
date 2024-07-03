using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace ProceduralGeneration
{
    public class TilemapGenerator : MonoBehaviour
    {
        private enum MapType
        {
            Gray,
            Water
        }

        [Title("Tilemap")]
        [SerializeField] [Required] private Tilemap tilemap;
        [SerializeField] [Required] public TileBase grassTile;
        [SerializeField] [Required] public TileBase waterTile;
        [SerializeField] [Required] public TileBase squareTile;

        [Title("Settings")]
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private bool useRandomSeed;
        [SerializeField] [DisableIf("useRandomSeed")] private int seed;
        [SerializeField] private MapType mapType;
        [SerializeField] [HideIf("mapType", MapType.Gray)] [OnValueChanged("GenerateMap")]
        [Range(0f, 1f)] private float waterProbability;
        [SerializeField] [HideIf("mapType", MapType.Gray)] [OnValueChanged("GenerateMap")]
        private bool singleWaterElimination;
        [SerializeField] [OnValueChanged("GenerateMap")]
        [Range(0f, 0.5f)] private float lacunarity;

        private float[,] _noiseMap;
        private float _minNoise = float.MaxValue;
        private float _maxNoise = float.MinValue;
        private float WaterThreshold => Mathf.Lerp(_minNoise, _maxNoise, waterProbability);

        [Title("Run")]
        [Button]
        public void GenerateMap()
        {
            GenerateNoiseMap();
            ClearTiles();
            switch (mapType)
            {
                case MapType.Gray:
                    SetGrayTiles();
                    break;
                case MapType.Water:
                    if (singleWaterElimination) EliminateSingleWater();
                    SetWaterTiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateNoiseMap()
        {
            if (useRandomSeed) seed = Time.time.GetHashCode();
            Random.InitState(seed);
            // init map
            _noiseMap = new float[width, height];
            // get random offset which will be used as the start point to generate noise map
            float offset = Random.Range(-9999f, 9999f);

            for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
            {
                float noise = PerlinNoise.GetNoise(w * lacunarity + offset, h * lacunarity + offset);
                // get min and max noise value to lerp
                if (noise < _minNoise) _minNoise = noise;
                if (noise > _maxNoise) _maxNoise = noise;
                _noiseMap[w, h] = noise;
            }
        }

        private void ClearTiles() => tilemap.ClearAllTiles();

        private void SetGrayTiles()
        {
            for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
            {
                tilemap.SetTile(new Vector3Int(w, h, 0), squareTile);
                var colorValue = Mathf.InverseLerp(_minNoise, _maxNoise, _noiseMap[w, h]);
                var color = Color.Lerp(Color.black, Color.white, colorValue);
                tilemap.SetTileFlags(new Vector3Int(w, h, 0), TileFlags.None);
                tilemap.SetColor(new Vector3Int(w, h, 0), color);
            }
        }

        private void SetWaterTiles()
        {
            for (int w = 0; w < width; w++)
            for (int h = 0; h < height; h++)
            {
                var tile = _noiseMap[w, h] < WaterThreshold ? waterTile : grassTile;
                tilemap.SetTile(new Vector3Int(w, h, 0), tile);
            }
        }

        private void EliminateSingleWater()
        {
            while (true)
            {
                bool hasSingleWater = false;
                for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                {
                    if (!(_noiseMap[w, h] < WaterThreshold) || !CheckAroundHavePairLand(w, h)) continue;
                    _noiseMap[w, h] = 1;
                    hasSingleWater = true;
                }

                if (!hasSingleWater) break;
            }
        }

        private bool CheckAroundHavePairLand(int x, int y)
        {
            bool left = false, right = false, up = false, down = false;
            if (x > 0) left = _noiseMap[x - 1, y] > WaterThreshold;
            if (x < width - 1) right = _noiseMap[x + 1, y] > WaterThreshold;
            if (y > 0) up = _noiseMap[x, y - 1] > WaterThreshold;
            if (y < height - 1) down = _noiseMap[x, y + 1] > WaterThreshold;
            return (left && right) || (up && down);
        }
    }
}