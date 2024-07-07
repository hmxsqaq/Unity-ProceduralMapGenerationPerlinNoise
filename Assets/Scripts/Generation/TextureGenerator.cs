using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration.Generation
{
    public class TextureGenerator : MapGeneratorBase
    {
        private enum DrawMode
        {
            HeightMap,
            ColorMap
        }

        [Title("Renderer")]
        [SerializeField] [Required] private Renderer textureRenderer;

        [Title("Settings")]
        [OnValueChanged(nameof(Generate))]
        [SerializeField] private DrawMode drawMode;
        [HideIf("drawMode", DrawMode.HeightMap)] [InlineEditor] [OnValueChanged(nameof(Generate))]
        [SerializeField] private TerrainConfiguration terrainConfiguration;

        protected override void Generate()
        {
            base.Generate();

            Texture2D texture = drawMode switch
            {
                DrawMode.HeightMap => GetTextureOfHeightMap(NoiseMap),
                DrawMode.ColorMap => GetTextureOfColorMap(NoiseMap, terrainConfiguration),
                _ => throw new ArgumentOutOfRangeException()
            };
            textureRenderer.sharedMaterial.mainTexture = texture;
        }

        private static Texture2D GetTextureOfColorMap(float[,] noiseMap, TerrainConfiguration terrainConfiguration)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainConfiguration.TerrainTypes.Count; i++)
                {
                    if (currentHeight > terrainConfiguration.TerrainTypes[i].height) continue;
                    colorMap[y * width + x] = terrainConfiguration.TerrainTypes[i].color;
                    break;
                }
            }
            return GetTexture(colorMap, width, height);
        }

        private static Texture2D GetTextureOfHeightMap(float[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            return GetTexture(colorMap, width, height);
        }

        private static Texture2D GetTexture(Color[] colorMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }
    }
}