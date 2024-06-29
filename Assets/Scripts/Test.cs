using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration
{
    public class Test : MonoBehaviour
    {
        [Button]
        public void Grad(int hash, double x, double y)
        {
            var h = hash & 7;
            var u = h < 4 ? x : y;
            var v = h < 4 ? y : x;
            Debug.Log("u: " + u + " v: " + v + " result: " + (((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v)));
        }

        [Button]
        public void Noise(float x, float y)
        {
            Debug.Log(PerlinNoise.Noise(x, y));
        }
    }
}