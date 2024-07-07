using UnityEngine;

namespace ProceduralGeneration.Noise
{
    public static class NoiseUtility
    {
        public static float[,] GetNoiseMap(int seed, int width, int height, float scale, Vector2 offset,
            int octaves, float persistance, float lacunarity)
        {
            Random.InitState(seed);
            // init map
            float[,] noiseMap = new float[width, height];
            // avoid scale <= 0
            scale = scale <= 0 ? 0.0001f : scale;
            // get offset point
            Vector2[] octavesOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
                octavesOffsets[i] = new Vector2(Random.Range(-9999f, 9999f), Random.Range(-9999f, 9999)) + offset;
            // store min and max value for normalization
            float minNoise = float.MaxValue;
            float maxNoise = float.MinValue;
            // store half width and height for centering the noise map
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noise = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octavesOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octavesOffsets[i].y;
                        float perlinNoise = PerlinNoise.GetNoise(sampleX, sampleY) * 2 - 1;
                        noise += perlinNoise * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
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