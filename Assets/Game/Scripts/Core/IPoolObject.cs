using UnityEngine;

namespace Game.Scripts.Core
{
    public interface IPoolObject
    {
        public bool IsSpawned { get; set; }

        GameObject GameObject { get; }
        
        void OnSpawn();

        void OnDespawn();
    }
}