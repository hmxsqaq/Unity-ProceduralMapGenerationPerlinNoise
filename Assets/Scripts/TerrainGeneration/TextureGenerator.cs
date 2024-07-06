using ProceduralGeneration.Noise;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TerrainGeneration
{
    public class TextureGenerator : MapGeneratorBase
    {
        [Title("Renderer")]
        [SerializeField] private Renderer textureRenderer;

        protected override void Generate()
        {
            base.Generate();
            Texture2D texture = TextureUtility.GetTextureFromHeightMap(NoiseMap);
            textureRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}