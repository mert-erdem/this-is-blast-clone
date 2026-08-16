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
        public event Action<Cannon> OnCannonRemoved;
        
        private Cannon[] _cannonSlots;
        
        private const float TWEEN_SLOT_DURATION = 1f;

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
                cannon.MoveToSlot(slots[i].position, TWEEN_SLOT_DURATION, () => OnCannonAdded?.Invoke(cannon));
                
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
                OnCannonRemoved?.Invoke(cannon);
                
                return;
            }
        }
        
        public IReadOnlyList<Cannon> CannonSlots => _cannonSlots;
    }
}
