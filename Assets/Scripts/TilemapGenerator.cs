using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        [Range(0.001f, 50f)] private float scale;

        private float[,] _noiseMap;

        [Title("Run")]
        [Button]
        public void GenerateMap()
        {
            if (useRandomSeed) seed = Time.time.GetHashCode();
            _noiseMap = NoiseUtility.GetNoiseMap(seed, width, height, scale);

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

        private void ClearTiles() => tilemap.ClearAllTiles();

        private void SetGrayTiles()
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    tilemap.SetTile(new Vector3Int(w, h, 0), squareTile);
                    var color = Color.Lerp(Color.black, Color.white, _noiseMap[w, h]);
                    tilemap.SetTileFlags(new Vector3Int(w, h, 0), TileFlags.None);
                    tilemap.SetColor(new Vector3Int(w, h, 0), color);
                }
            }
        }

        private void SetWaterTiles()
        {
            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    var tile = _noiseMap[w, h] < waterProbability ? waterTile : grassTile;
                    tilemap.SetTile(new Vector3Int(w, h, 0), tile);
                }
            }
        }

        private void EliminateSingleWater()
        {
            while (true)
            {
                bool hasSingleWater = false;
                for (int w = 0; w < width; w++)
                {
                    for (int h = 0; h < height; h++)
                    {
                        if (!(_noiseMap[w, h] < waterProbability) || !CheckAroundHavePairLand(w, h)) continue;
                        _noiseMap[w, h] = 1;
                        hasSingleWater = true;
                    }
                }

                if (!hasSingleWater) break;
            }
        }

        private bool CheckAroundHavePairLand(int x, int y)
        {
            bool left = false, right = false, up = false, down = false;
            if (x > 0) left = _noiseMap[x - 1, y] > waterProbability;
            if (x < width - 1) right = _noiseMap[x + 1, y] > waterProbability;
            if (y > 0) up = _noiseMap[x, y - 1] > waterProbability;
            if (y < height - 1) down = _noiseMap[x, y + 1] > waterProbability;
            return (left && right) || (up && down);
        }
    }
}