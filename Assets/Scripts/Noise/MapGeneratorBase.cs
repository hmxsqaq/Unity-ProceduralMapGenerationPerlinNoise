using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration.Noise
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
    }
}