using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralGeneration.Generation
{
    [Serializable]
    public struct TerrainType
    {
        public string name;
        [Range(0f, 1f)] public float height;
        public Color color;
    }

    [CreateAssetMenu(menuName = "Procedural Generation/Terrain Configuration")]
    public class TerrainConfiguration : ScriptableObject
    {
        [SerializeField] [TableList] private List<TerrainType> terrainTypes;

        public List<TerrainType> TerrainTypes => terrainTypes;

        private void OnValidate()
        {
            terrainTypes.Sort((a, b) => a.height.CompareTo(b.height));
        }
    }
}