using Game.ScriptableObjects;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Entities
{
    public class Cannon : MonoBehaviour, IPoolObject
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BlockColorSo blockColorSo;
        
        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        private BlockColor _color;
        private int _ammo;
    
        // Will be used by ObjectPool
        public void Initialize(CannonData data)
        {
            _color = data.color;
            _ammo = data.ammo;
            
            Paint(_color);
        }

        private void Fire(Vector3 targetPos)
        {
            // TODO: Animation and etc.
        }

        private void Paint(BlockColor blockColor)
        {
            meshRenderer.sharedMaterial = blockColorSo.GetMaterial(blockColor);
        }
        
        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }
    }
}
