using System;
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

        public bool IsReserved { get; private set; }
        
        public event Action<TargetBlock> OnDestroyed;
        
        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        private BlockColor _color;
        private int _health;
        private Vector2Int _gridPosition;
    
        // Will be used by ObjectPool
        public void Initialize(TargetBlockData data, Vector2Int gridPosition)
        {
            _color = data.color;
            _health = data.health;
            _gridPosition = gridPosition;
            
            Paint(_color);
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;

            if (_health <= 0)
            {
                OnDestroyed?.Invoke(this);
            }
        }

        public Vector2Int GetGridPosition()
        {
            return _gridPosition;
        }

        public BlockColor GetColor()
        {
            return _color;
        }

        public void Reserve()
        {
            IsSpawned = true;
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
