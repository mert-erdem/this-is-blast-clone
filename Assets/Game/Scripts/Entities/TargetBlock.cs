using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Entities
{
    public class TargetBlock : MonoBehaviour, IPoolObject
    {
        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        private BlockColor _color;
        private int _health;
    
        // Will be used by ObjectPool
        public void Initialize(TargetBlockData data)
        {
            _color = data.color;
            _health = data.health;
            
            // Paint(Helper.ToUnityColor(_color));
        }
        
        private void Die(){}

        private void Paint(Color color)
        {
            
        }
        
        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }
    }
}
