using UnityEngine;

namespace DevLib.DatabaseSystem.Runtime
{
    public class IndexedAsset : ScriptableObject
    {
        [field: SerializeField] public int AssetIndex { get; set; }
        [field: SerializeField] public string AssetName { get; set; }
    }
}