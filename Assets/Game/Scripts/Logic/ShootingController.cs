using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Core;
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
        
        private const float FIRE_INTERVAL = 0.2f;
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
                bool isFireComplete = false;
                cannon.Fire(target, () => isFireComplete = true);
                yield return new WaitUntil(() => isFireComplete || cannon == null || !cannon.IsSpawned);

                if (cannon != null && cannon.IsSpawned && cannon.HasAmmo)
                    yield return FireIntervalWait;
            }

            _firingCannons.Remove(cannon);

            if (cannon != null && cannon.IsSpawned && cannon.HasAmmo)
                cannon.PlayInitialLookTween();

            if (cannon != null && !cannon.HasAmmo)
                cannonSlotManager.RemoveCannon(cannon);

            CheckDeadlock();
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

            CheckDeadlock();
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

        /// <summary>
        /// Checking the loss condition when all slots are filled and
        /// no front target matches any slotted cannon color.
        /// </summary>
        private void CheckDeadlock()
        {
            if (_firingCannons.Count > 0)
                return;

            if (!board.HasTargetBlocks())
                return;

            if (board.HasMovingBlocks())
                return;

            if (!cannonSlotManager.AreAllSlotsFilled())
                return;

            if (board.HasFireableFrontBlockMatching(cannonSlotManager.CannonSlots))
                return;

            // LOSS CONDITION
            GameManager.ActionGameOver?.Invoke();
        }

        private void OnDestroy()
        {
            cannonSlotManager.OnCannonAdded -= TryFire;
            board.OnBoardStateChanged -= ReevaluateCannons;
        }
    }
}
