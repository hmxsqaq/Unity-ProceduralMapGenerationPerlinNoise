using UnityEngine;

namespace ProceduralGeneration.Utility
{
    public static class TextureUtility
    {
        public static Texture2D GetTextureFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[] colorMap = new Color[width * height];
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            return GetTextureFromColorMap(colorMap, width, height);
        }

        public static Texture2D GetTextureFromColorMap(Color[] colorMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }
    }
}