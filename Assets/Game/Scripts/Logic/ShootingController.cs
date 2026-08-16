using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Entities;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class ShootingController : MonoBehaviour
    {
        // They are singletons (to see dependencies, DI container can be added later)
        [SerializeField] private Board board;
        [SerializeField] private CannonSlotManager cannonSlotManager;
        
        private const float FIRE_INTERVAL = 0.05f;
        private static readonly WaitForSeconds FireIntervalWait = new(FIRE_INTERVAL);
        
        private readonly HashSet<Cannon> _firingCannons = new();

        private void Awake()
        {
            cannonSlotManager.OnCannonAdded += TryFire;
            board.OnBoardStateChanged += ReevaluateCannons;
        }

        private void TryFire(Cannon cannon)
        {
            if (cannon == null || !cannon.HasAmmo || !cannon.IsReadyToFire)
                return;

            if (_firingCannons.Contains(cannon))
                return;

            StartCoroutine(FireRoutine(cannon));
        }

        // WaitForSeconds used due to achieve firing delta per cannon.
        private IEnumerator FireRoutine(Cannon cannon)
        {
            _firingCannons.Add(cannon);

            while (cannon != null &&
                   cannon.HasAmmo &&
                   cannon.IsReadyToFire &&
                   board.TryGetTarget(cannon.GetColor(), out TargetBlock target))
            {
                cannon.Fire(target);
                yield return FireIntervalWait;
            }

            _firingCannons.Remove(cannon);

            if (cannon != null && !cannon.HasAmmo)
                cannonSlotManager.RemoveCannon(cannon);
        }

        private void ReevaluateCannons()
        {
            for (int i = 0; i < cannonSlotManager.CannonSlots.Count; i++)
            {
                Cannon cannon = cannonSlotManager.CannonSlots[i];

                if (cannon == null || !cannon.HasAmmo || !cannon.IsReadyToFire)
                    continue;

                if (HasEarlierSameColorCannon(i, cannon.GetColor()))
                    continue;

                TryFire(cannon);
            }
        }

        /// <summary>
        /// To achieve priority between same color cannons.
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private bool HasEarlierSameColorCannon(int currentIndex, BlockColor color)
        {
            for (int i = 0; i < currentIndex; i++)
            {
                Cannon cannon = cannonSlotManager.CannonSlots[i];

                if (cannon == null || !cannon.HasAmmo)
                    continue;

                if (cannon.GetColor() == color)
                    return true;
            }

            return false;
        }

        private void OnDestroy()
        {
            cannonSlotManager.OnCannonAdded -= TryFire;
            board.OnBoardStateChanged -= ReevaluateCannons;
        }
    }
}
