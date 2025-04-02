using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration.Generation
{
    public class TerrainGeneratorWithShader : MapGeneratorBase
    {
        [Title("Renderer")]
        [SerializeField] [Required] private MeshFilter meshFilter;
        [SerializeField] [Required] private MeshRenderer meshRenderer;

        [Title("Mesh Settings")]
        [Range(1f, 50f)] [OnValueChanged(nameof(Generate))]
        [SerializeField] private float meshHeightMultiplier;
        [OnValueChanged(nameof(Generate))]
        [SerializeField] private AnimationCurve meshHeightCurve;

        [Title("Texture Settings")]
        [Required] [InlineEditor] [OnValueChanged(nameof(Generate))]
        [SerializeField] private TerrainConfiguration terrainConfiguration;

        protected override void Generate()
        {
            base.Generate();

            Texture2D texture = TextureGenerator.GetTextureOfColorMap(NoiseMap, terrainConfiguration);
            meshFilter.sharedMesh = GenerateTerrainMesh(NoiseMap, meshHeightMultiplier, meshHeightCurve);
            meshRenderer.material.SetTexture("_BlendTex", texture);
        }

        private static Mesh GenerateTerrainMesh(float[,] noiseMap, float heightMultiplier, AnimationCurve heightCurve)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);
            // get offset to center the mesh
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 1) / 2f;

            MeshData meshData = new MeshData(width, height);
            int vertexIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier, topLeftZ - y);
                    meshData.UVs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }
                    vertexIndex++;
                }
            }
            return meshData.CreateMesh();
        }

        private struct MeshData
        {
            public readonly Vector3[] Vertices;
            public readonly int[] Triangles;
            public readonly Vector2[] UVs;
            public int TriangleIndex;

            public MeshData(int width, int height)
            {
                Vertices = new Vector3[width * height];
                Triangles = new int[(width - 1) * (height - 1) * 6];
                UVs = new Vector2[width * height];
                TriangleIndex = 0;
            }

            public void AddTriangle(int a, int b, int c)
            {
                Triangles[TriangleIndex] = a;
                Triangles[TriangleIndex + 1] = b;
                Triangles[TriangleIndex + 2] = c;
                TriangleIndex += 3;
            }

            public Mesh CreateMesh()
            {
                Mesh mesh = new Mesh
                {
                    vertices = Vertices,
                    triangles = Triangles,
                    uv = UVs
                };
                mesh.RecalculateNormals();
                return mesh;
            }
        }
    }
}