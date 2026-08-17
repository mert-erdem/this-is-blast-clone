using System;
using DG.Tweening;
using Game.ScriptableObjects;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Enums;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Entities
{
    public class Cannon : MonoBehaviour, IPoolObject
    {
        [SerializeField] private Transform transformVisual;
        [SerializeField] private TextMeshPro textAmmo;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BlockColorSo blockColorSo;
        
        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        public bool HasAmmo => _ammo > 0;
        public bool IsReadyToFire { get; private set; }

        private const int DAMAGE = 1;
        private const float REMOVE_TWEEN_OVERSHOOT = 2.5f;
        private const float QUEUE_SHIFT_TWEEN_OVERSHOOT = 2f;
        private const float LOOK_TWEEN_DURATION = 0.5f;
        private const float RETURN_LOOK_TWEEN_DURATION = 0.25f;
        
        private BlockColor _color;
        private int _ammo;

        // Tween Related
        private Tween _slotTween, _queueTween, _lookTween;
        private Vector3 _initialScale;
        private Quaternion _initialVisualRotation;
    
        public void Initialize(CannonData data)
        {
            _slotTween?.Kill();
            _slotTween = null;
            _queueTween?.Kill();
            _queueTween = null;
            _lookTween?.Kill();
            _lookTween = null;
            IsReadyToFire = false;
            transform.localScale = _initialScale;
            
            _color = data.color;
            _ammo = data.ammo;
            
            Paint(_color);
            SetAmmoText(_ammo);
        }

        public BlockColor GetColor()
        {
            return _color;
        }
        
        public int GetAmmo()
        {
            return _ammo;
        }

        public void Fire(TargetBlock targetBlock, Action onComplete = null)
        {
            if (targetBlock == null || !HasAmmo || !IsReadyToFire)
            {
                onComplete?.Invoke();
                return;
            }

            IsReadyToFire = false;
            LookTargetTween(targetBlock.GetPosition(), () =>
            {
                if (targetBlock != null && targetBlock.IsFireable && HasAmmo)
                {
                    _ammo--;
                    targetBlock.TakeDamage(DAMAGE);
                    SetAmmoText(_ammo);
                }

                IsReadyToFire = HasAmmo;
                onComplete?.Invoke();
            });
        }

        public void MoveToSlot(Vector3 slotPosition, float duration, Action onComplete = null)
        {
            PlaySlotRepositionTween(slotPosition, duration, onComplete);
        }

        public void PlayQueueShiftTween(Vector3 queuePosition, float duration, Action onComplete = null)
        {
            _queueTween?.Kill();
            _queueTween = null;

            IsReadyToFire = false;
            transform.localScale = _initialScale;

            if (duration <= 0f)
            {
                transform.position = queuePosition;
                onComplete?.Invoke();

                return;
            }

            _queueTween = transform
                .DOMove(queuePosition, duration)
                .SetEase(Ease.OutCubic, QUEUE_SHIFT_TWEEN_OVERSHOOT)
                .OnComplete(() =>
                {
                    _queueTween = null;
                    IsReadyToFire = false;
                    onComplete?.Invoke();
                });
        }

        private void PlaySlotRepositionTween(Vector3 position, float duration, Action onComplete = null)
        {
            _slotTween?.Kill();
            _slotTween = null;
            _queueTween?.Kill();
            _queueTween = null;

            IsReadyToFire = false;

            if (duration <= 0f)
            {
                transform.position = position;
                transform.localScale = _initialScale;
                IsReadyToFire = true;
                onComplete?.Invoke();

                return;
            }

            float scaleDownDuration = duration * 0.35f;
            float scaleUpDuration = duration - scaleDownDuration;

            _slotTween = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, scaleDownDuration).SetEase(Ease.InBack))
                .AppendCallback(() => transform.position = position)
                .Append(transform.DOScale(_initialScale, scaleUpDuration).SetEase(Ease.OutElastic))
                .OnComplete(() =>
                {
                    _slotTween = null;
                    IsReadyToFire = true;
                    onComplete?.Invoke();
                });
        }

        public void PlayRemoveFromSlotTween(float duration, Action onComplete = null)
        {
            _slotTween?.Kill();
            _slotTween = null;
            _queueTween?.Kill();
            _queueTween = null;
            _lookTween?.Kill();
            _lookTween = null;
            IsReadyToFire = false;

            if (duration <= 0f)
            {
                transform.localScale = Vector3.zero;
                onComplete?.Invoke();

                return;
            }

            _slotTween = transform
                .DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack, REMOVE_TWEEN_OVERSHOOT)
                .OnComplete(() =>
                {
                    _slotTween = null;
                    onComplete?.Invoke();
                });
        }

        // For cannons that has ammo but no target to shoot.
        public void PlayInitialLookTween()
        {
            // If already looking at initial rotation
            if (Quaternion.Angle(transformVisual.localRotation, _initialVisualRotation) <= 0.1f)
                return;

            _lookTween?.Kill();
            _lookTween = null;

            _lookTween = transformVisual
                .DOLocalRotateQuaternion(_initialVisualRotation, RETURN_LOOK_TWEEN_DURATION)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _lookTween = null);
        }

        private void LookTargetTween(Vector3 targetPosition, Action onComplete = null)
        {
            _lookTween?.Kill();
            _lookTween = null;

            _lookTween = transformVisual
                .DOLookAt(targetPosition, LOOK_TWEEN_DURATION)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _lookTween = null;
                    onComplete?.Invoke();
                });
        }

        private void Paint(BlockColor blockColor)
        {
            meshRenderer.sharedMaterial = blockColorSo.GetMaterial(blockColor);
        }

        private void SetAmmoText(int ammo)
        {
            textAmmo.text = ammo.ToString();
        }
        
        public void OnSpawn()
        {
            if (_initialScale == Vector3.zero)
                _initialScale = transform.localScale;

            _slotTween?.Kill();
            _slotTween = null;
            _queueTween?.Kill();
            _queueTween = null;
            _lookTween?.Kill();
            _lookTween = null;

            _initialVisualRotation = transformVisual.localRotation;

            transform.localScale = _initialScale;
            transformVisual.localRotation = _initialVisualRotation;
            IsReadyToFire = false;
        }

        public void OnDespawn()
        {
            _slotTween?.Kill();
            _slotTween = null;
            _queueTween?.Kill();
            _queueTween = null;
            _lookTween?.Kill();
            _lookTween = null;
            transform.localScale = _initialScale;
            transformVisual.localRotation = _initialVisualRotation;
            IsReadyToFire = false;
        }
    }
}
