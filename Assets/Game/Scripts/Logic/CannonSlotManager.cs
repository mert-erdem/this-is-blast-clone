using System;
using System.Collections.Generic;
using Game.Scripts.Core;
using Game.Scripts.Entities;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class CannonSlotManager : Singleton<CannonSlotManager>
    {
        [SerializeField] private Transform[] slots;
        
        public event Action<Cannon> OnCannonAdded;
        
        private Cannon[] _cannonSlots;

        public void Initialize()
        {
            _cannonSlots = new Cannon[slots.Length];
        }

        public bool TryAddCannon(Cannon cannon)
        {
            for (int i = 0; i < _cannonSlots.Length; i++)
            {
                if (_cannonSlots[i] != null)
                    continue;

                _cannonSlots[i] = cannon;
                cannon.transform.position = slots[i].position;

                OnCannonAdded?.Invoke(cannon);
                
                return true;
            }

            return false;
        }

        public void RemoveCannon(Cannon cannon)
        {
            for (int i = 0; i < _cannonSlots.Length; i++)
            {
                if (_cannonSlots[i] != cannon)
                    continue;

                _cannonSlots[i] = null;
                return;
            }
            
            // TODO: Remove cannon via CannonManager
        }
        
        public IReadOnlyList<Cannon> CannonSlots => _cannonSlots;
    }
}
