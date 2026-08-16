using Game.Scripts.Core;
using Game.Scripts.Entities;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class CannonSlotManager : Singleton<CannonSlotManager>
    {
        [SerializeField] private Transform[] slots;
        private Cannon[] _activeCannons;

        public void Initialize()
        {
            _activeCannons = new Cannon[slots.Length];
        }

        public bool TryAddCannon(Cannon cannon)
        {
            for (int i = 0; i < _activeCannons.Length; i++)
            {
                if (_activeCannons[i] != null)
                    continue;

                _activeCannons[i] = cannon;
                cannon.transform.position = slots[i].position;

                return true;
            }

            return false;
        }

        public void RemoveCannon(Cannon cannon)
        {
            for (int i = 0; i < _activeCannons.Length; i++)
            {
                if (_activeCannons[i] != cannon)
                    continue;

                _activeCannons[i] = null;
                return;
            }
        }
    }
}
