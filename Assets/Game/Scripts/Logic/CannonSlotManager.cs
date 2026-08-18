using System;
using System.Collections.Generic;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Entities;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class CannonSlotManager : Singleton<CannonSlotManager>
    {
        [SerializeField] private Transform[] slots;
        
        public IReadOnlyList<Cannon> CannonSlots => _cannonSlots;
        
        public event Action<Cannon> OnCannonAdded;
        public event Action<Cannon> OnCannonRemoved;
        
        private Cannon[] _cannonSlots;
        
        // Tween Related
        private const float TWEEN_SLOT_DURATION = 0.5f;
        
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

        public bool AreAllSlotsFilled()
        {
            for (int i = 0; i < _cannonSlots.Length; i++)
            {
                if (_cannonSlots[i] == null)
                    return false;
            }

            return true;
        }

        #region Saving/Restoring

        public CannonSlotSaveData[] GetSaveData()
        {
            List<CannonSlotSaveData> saveData = new();

            for (int i = 0; i < _cannonSlots.Length; i++)
            {
                Cannon cannon = _cannonSlots[i];

                if (cannon == null)
                    continue;

                saveData.Add(new CannonSlotSaveData
                {
                    slotIndex = i,
                    color = cannon.GetColor(),
                    ammo = cannon.GetAmmo()
                });
            }

            return saveData.ToArray();
        }
        
        public void SetSlotFromSave(int slotIndex, Cannon cannon)
        {
            if (slotIndex < 0 || slotIndex >= _cannonSlots.Length)
                return;

            _cannonSlots[slotIndex] = cannon;
            cannon.MoveToSlot(slots[slotIndex].position, 0f, () => OnCannonAdded?.Invoke(cannon));
        }

        #endregion
    }
}
