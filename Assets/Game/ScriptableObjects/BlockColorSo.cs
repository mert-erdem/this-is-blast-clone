using Game.Scripts.Enums;
using UnityEngine;

namespace Game.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BlockColorSo", menuName = "Scriptable Objects/BlockColorSo")]
    public class BlockColorSo : ScriptableObject
    {
        [SerializeField] private Material[] materials;

        public Material GetMaterial(BlockColor color)
        {
            return materials[(int)color];
        }
    }
}
