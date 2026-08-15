using Game.Scripts.Common;
using Game.Scripts.Data;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Entities
{
    public class Cannon : MonoBehaviour
    {
        private BlockColor _color;
        private int _ammo;
    
        // Will be used by ObjectPool
        public void Initialize(CannonData data)
        {
            _color = data.color;
            _ammo = data.ammo;
            
            // Paint(Helper.ToUnityColor(_color));
        }

        private void Fire(Vector3 targetPos)
        {
            // TODO: Animation and etc.
        }

        private void Paint(Color color)
        {
            
        }
    }
}
