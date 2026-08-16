using Game.ScriptableObjects;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Entities
{
    public class TargetBlock : MonoBehaviour, IPoolObject
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BlockColorSo blockColorSo;
        
        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        private BlockColor _color;
        private int _health;
    
        // Will be used by ObjectPool
        public void Initialize(TargetBlockData data)
        {
            _color = data.color;
            _health = data.health;
            
            Paint(_color);
        }
        
        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }
        
        private void Die(){}

        private void Paint(BlockColor blockColor)
        {
            meshRenderer.sharedMaterial = blockColorSo.GetMaterial(blockColor);
        }
    }
}
