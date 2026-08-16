using Game.Scripts.Entities;
using Game.Scripts.Logic;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask cannonLayer;

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                TrySelectCannon(Input.mousePosition);
            }
#elif UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount <= 0)
                return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                TrySelectCannon(touch.position);
            }
#endif
        }

        // MAIN PIPELINE STARTS HERE!
        private void TrySelectCannon(Vector2 screenPosition)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, cannonLayer))
                return;

            Cannon cannon = hit.collider.GetComponentInParent<Cannon>();

            if (cannon != null)
            {
                CannonManager.Instance.TrySelect(cannon);
            }
        }
    }
}