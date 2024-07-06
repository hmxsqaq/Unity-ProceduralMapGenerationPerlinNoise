using System.Collections.Generic;
using UnityEngine;

namespace ProceduralGeneration.Noise
{
    public static class PerlinNoise
    {
        private static readonly List<int> Permutation256 = new(256)
        {
            151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37,
            240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177,
            33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146,
            158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25,
            63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100,
            109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206,
            59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
            101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218,
            246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
            49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205,
            93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
        };

        private static readonly List<int> Permutation512 = new(512);

        static PerlinNoise()
        {
            Permutation512.AddRange(Permutation256);
            Permutation512.AddRange(Permutation256);
        }

        /// <summary>
        /// Regenerate the permutation list.
        /// </summary>
        public static void RegeneratePermutation()
        {
            for (var i = Permutation256.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (Permutation256[i], Permutation256[j]) = (Permutation256[j], Permutation256[i]);
            }
            Permutation512.Clear();
            Permutation512.AddRange(Permutation256);
            Permutation512.AddRange(Permutation256);
        }

        /// <summary>
        /// Get the noise value of 2D perlin noise.
        /// </summary>
        /// <returns>[0, 1]</returns>
        public static float GetNoise(float x, float y)
        {
            // the grid cell coordinates
            int gridX = Floor(x) & 255;
            int gridY = Floor(y) & 255;
            // the relative coordinates of the point in the cell
            float dx = x - Floor(x);
            float dy = y - Floor(y);
            // fade the relative coordinates
            float u = Fade(dx);
            float v = Fade(dy);
            // hash coordinates of the 4 corners
            int hashA = Permutation512[gridX] + gridY;
            int hashB = Permutation512[gridX + 1] + gridY;
            // bilinear interpolation
            float y0 = Lerp(Grad(Permutation512[hashA], dx, dy), Grad(Permutation512[hashB], dx - 1, dy), u);
            float y1 = Lerp(Grad(Permutation512[hashA + 1], dx, dy - 1), Grad(Permutation512[hashB + 1], dx - 1, dy - 1), u);
            return Lerp(y0, y1, v);
        }

        /// <summary>
        /// Get the noise value of 3D perlin noise.
        /// </summary>
        /// <returns>[0, 1]</returns>
        public static float GetNoise(float x, float y, float z)
        {
            int gridX = Floor(x) & 255, gridY = Floor(y) & 255, gridZ = Floor(z) & 255;
            float dx = x - Floor(x), dy = y - Floor(y), dz = z - Floor(z);
            float u = Fade(dx), v = Fade(dy), w = Fade(dz);
            int hashA = Permutation512[gridX] + gridY,
                hashAA = Permutation512[hashA] + gridZ,
                hashAB = Permutation512[hashA + 1] + gridZ;
            int hashB = Permutation512[gridX + 1] + gridY,
                hashBA = Permutation512[hashB] + gridZ,
                hashBB = Permutation512[hashB + 1] + gridZ;
            return
                Lerp(
                    Lerp(
                        Lerp(
                            Grad(Permutation512[hashAA], x, y, z),
                            Grad(Permutation512[hashBA], x - 1, y, z),
                            u),
                        Lerp(
                            Grad(Permutation512[hashAB], x, y - 1, z),
                            Grad(Permutation512[hashBB], x - 1, y - 1, z),
                            u),
                        v),
                    Lerp(
                        Lerp(
                            Grad(Permutation512[hashAA + 1], x, y, z - 1),
                            Grad(Permutation512[hashBA + 1], x - 1, y, z - 1),
                            u),
                        Lerp(
                            Grad(Permutation512[hashAB + 1], x, y - 1, z - 1),
                            Grad(Permutation512[hashBB + 1], x - 1, y - 1, z - 1),
                            u),
                        v),
                    w);
        }

        /// <summary>
        /// Get the floor value of x
        /// </summary>
        private static int Floor(float x) => x > 0 ? (int)x : (int)x - 1;

        /// <summary>
        /// Linear interpolation
        /// </summary>
        /// <param name="a">value1</param>
        /// <param name="b">value2</param>
        /// <param name="t">weight</param>
        private static float Lerp(float a, float b, float t) => a + (b - a) * t;

        /// <summary>
        /// Fade: 6x^5 - 15x^4 + 10x^3
        /// </summary>
        private static float Fade(float x) => x * x * x * (x * (x * 6 - 15) + 10);

        /// <summary>
        /// Get 2d gradient value
        /// </summary>
        /// <returns>(x + y) or (y - x) or (x - y) or (-x - y)</returns>
        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 7; // convert the hash to 0-7
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return (h & 1) == 0 ? u : -u + (h & 2) == 0 ? v : -v;
        }

        /// <summary>
        /// Get 3d gradient value
        /// </summary>
        private static float Grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : h is 12 or 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}