using ProceduralGeneration.Noise;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration.Generation
{
    public abstract class MapGeneratorBaseSingleNoise : MonoBehaviour
    {
        [Title("Noise Map")]
        [OnValueChanged(nameof(Generate))] [Range(1, 300)]
        [SerializeField] protected int width;
        [OnValueChanged(nameof(Generate))] [Range(1, 300)]
        [SerializeField] protected int height;
        [SerializeField] protected bool useRandomSeed;
        [DisableIf("useRandomSeed")]
        [SerializeField] protected int seed;
        [Range(0.001f, 500f)] [OnValueChanged("Generate")]
        [SerializeField] protected float scale;
        [OnValueChanged("Generate")]
        [SerializeField] protected Vector2 offset;

        protected float[,] NoiseMap;

        [Title("Run")]
        [Button]
        protected virtual void Generate()
        {
            if (useRandomSeed) seed = Time.time.GetHashCode();
            NoiseMap = NoiseUtility.GetNoiseMap(seed, width, height, scale, offset);
        }

        private void OnValidate()
        {
            if (scale < 0.001f) scale = 0.001f;
            if (scale > 500f) scale = 500f;
        }
    }
}