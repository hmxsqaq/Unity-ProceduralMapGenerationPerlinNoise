using UnityEngine;

namespace ProceduralGeneration.Noise
{
    public static class NoiseUtility
    {
        public static float[,] GetNoiseMap(int seed, int width, int height, float scale)
        {
            Random.InitState(seed);
            // init map
            float[,] noiseMap = new float[width, height];
            // avoid scale <= 0
            scale = scale <= 0 ? 0.0001f : scale;
            // get offset point
            Vector2 offset = new Vector2(Random.Range(-9999f, 9999f), Random.Range(-9999f, 9999));
            // store min and max value for normalization
            float minNoise = float.MaxValue;
            float maxNoise = float.MinValue;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float sampleX = x / scale + offset.x;
                    float sampleY = y / scale + offset.y;
                    float noise = PerlinNoise.GetNoise(sampleX, sampleY);
                    // get min and max noise value to lerp
                    if (noise < minNoise) minNoise = noise;
                    if (noise > maxNoise) maxNoise = noise;
                    noiseMap[x, y] = noise;
                }
            }

            // normalize the noise map
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    noiseMap[x, y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x, y]);

            return noiseMap;
        }
    }
}