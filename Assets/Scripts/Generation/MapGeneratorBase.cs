using ProceduralGeneration.Noise;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration.Generation
{
    public abstract class MapGeneratorBase : MonoBehaviour
    {
        [Title("Noise Map")]
        [OnValueChanged(nameof(Generate))]
        [SerializeField] protected int width;
        [OnValueChanged(nameof(Generate))]
        [SerializeField] protected int height;
        [SerializeField] protected bool useRandomSeed;
        [DisableIf("useRandomSeed")]
        [SerializeField] protected int seed;
        [Range(0.001f, 500f)] [OnValueChanged("Generate")]
        [SerializeField] protected float scale;
        [OnValueChanged("Generate")]
        [SerializeField] protected Vector2 offset;
        [OnValueChanged("Generate")]
        [SerializeField] protected int octaves;
        [OnValueChanged("Generate")] [Range(0f, 1f)]
        [SerializeField] protected float persistance;
        [OnValueChanged("Generate")] [Range(1f, 10f)]
        [SerializeField] protected float lacunarity;

        protected float[,] NoiseMap;

        [Title("Run")]
        [Button]
        protected virtual void Generate()
        {
            if (useRandomSeed) seed = Time.time.GetHashCode();
            NoiseMap = NoiseUtility.GetNoiseMap(seed, width, height, scale, offset, octaves, persistance, lacunarity);
        }

        private void OnValidate()
        {
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            if (scale < 0.001f) scale = 0.001f;
            if (scale > 500f) scale = 500f;
            if (octaves < 1) octaves = 1;
            if (persistance < 0f) persistance = 0f;
            if (persistance > 1f) persistance = 1f;
            if (lacunarity < 1f) lacunarity = 1f;
        }
    }
}