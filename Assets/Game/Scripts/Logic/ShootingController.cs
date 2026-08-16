using Game.Scripts.Entities;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class ShootingController : MonoBehaviour
    {
        // They are singletons (to see dependencies, DI container can be added later)
        [SerializeField] private Board board;
        [SerializeField] private CannonSlotManager cannonSlotManager;

        private void Awake()
        {
            cannonSlotManager.OnCannonAdded += TryFire;
            board.OnBoardStateChanged += ReevaluateCannons;
        }

        private void TryFire(Cannon cannon)
        {
            if (cannon == null)
                return;
            
            if (!board.TryGetTarget(cannon.GetColor(), out TargetBlock target))
                return;

            cannon.Fire(target);
        }

        private void ReevaluateCannons()
        {
            foreach (Cannon cannon in cannonSlotManager.CannonSlots)
            {
                if (cannon == null)
                    continue;
                
                TryFire(cannon);
            }
        }

        private void OnDestroy()
        {
            cannonSlotManager.OnCannonAdded -= TryFire;
            board.OnBoardStateChanged -= ReevaluateCannons;
        }
    }
}
