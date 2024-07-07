using ProceduralGeneration.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration.Generation
{
    public abstract class MapGeneratorBase : MonoBehaviour
    {
        [Title("Noise Map")]
        [SerializeField] protected int width;
        [SerializeField] protected int height;
        [SerializeField] protected bool useRandomSeed;
        [SerializeField] [DisableIf("useRandomSeed")] protected int seed;
        [SerializeField] [OnValueChanged("Generate")] [Range(0.001f, 500f)] protected float scale;

        protected float[,] NoiseMap;

        [Title("Run")]
        [Button]
        protected virtual void Generate()
        {
            if (useRandomSeed) seed = Time.time.GetHashCode();
            NoiseMap = NoiseUtility.GetNoiseMap(seed, width, height, scale);
        }

        private void OnValidate()
        {
            if (width < 1) width = 1;
            if (height < 1) height = 1;
            if (scale < 0.001f) scale = 0.001f;
            if (scale > 500f) scale = 500f;
        }
    }
}