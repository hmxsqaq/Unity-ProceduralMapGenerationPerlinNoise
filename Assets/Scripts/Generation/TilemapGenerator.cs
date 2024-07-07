using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ProceduralGeneration.Generation
{
    public class TilemapGenerator : MapGeneratorBase
    {
        private enum DrawMode
        {
            Gray,
            Color
        }

        [Title("Tilemap")]
        [SerializeField] [Required] private Tilemap tilemap;
        [SerializeField] [Required] private TileBase grassTile;
        [SerializeField] [Required] private TileBase waterTile;
        [SerializeField] [Required] private TileBase squareTile;

        [Title("Settings")]
        [SerializeField] private DrawMode drawMode;
        [SerializeField] [HideIf("drawMode", DrawMode.Gray)] [OnValueChanged(nameof(Generate))]
        [Range(0f, 1f)] private float waterProbability;
        [SerializeField] [HideIf("drawMode", DrawMode.Gray)] [OnValueChanged(nameof(Generate))]
        private bool singleWaterElimination;

        protected override void Generate()
        {
            base.Generate();

            ClearTiles();
            switch (drawMode)
            {
                case DrawMode.Gray:
                    SetGrayTiles();
                    break;
                case DrawMode.Color:
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
                    var color = Color.Lerp(Color.black, Color.white, NoiseMap[w, h]);
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
                    var tile = NoiseMap[w, h] < waterProbability ? waterTile : grassTile;
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
                        if (!(NoiseMap[w, h] < waterProbability) || !CheckAroundHavePairLand(w, h)) continue;
                        NoiseMap[w, h] = 1;
                        hasSingleWater = true;
                    }
                }

                if (!hasSingleWater) break;
            }
        }

        private bool CheckAroundHavePairLand(int x, int y)
        {
            bool left = false, right = false, up = false, down = false;
            if (x > 0) left = NoiseMap[x - 1, y] > waterProbability;
            if (x < width - 1) right = NoiseMap[x + 1, y] > waterProbability;
            if (y > 0) up = NoiseMap[x, y - 1] > waterProbability;
            if (y < height - 1) down = NoiseMap[x, y + 1] > waterProbability;
            return (left && right) || (up && down);
        }
    }
}