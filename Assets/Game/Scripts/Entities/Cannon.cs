using System;
using DG.Tweening;
using Game.ScriptableObjects;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Entities
{
    public class Cannon : MonoBehaviour, IPoolObject
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BlockColorSo blockColorSo;
        
        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        public bool HasAmmo => _ammo > 0;
        public bool IsReadyToFire { get; private set; }

        private const int DAMAGE = 1;
        
        private BlockColor _color;
        private int _ammo;
        private Tween _slotTween;
        private Vector3 _initialScale;
    
        public void Initialize(CannonData data)
        {
            _slotTween?.Kill();
            _slotTween = null;
            IsReadyToFire = false;
            transform.localScale = _initialScale;
            
            _color = data.color;
            _ammo = data.ammo;
            
            Paint(_color);
        }

        public BlockColor GetColor()
        {
            return _color;
        }

        public void Fire(TargetBlock targetBlock)
        {
            if (targetBlock == null || !HasAmmo || !IsReadyToFire)
                return;

            _ammo--;

            targetBlock.TakeDamage(DAMAGE);
        }

        public void MoveToSlot(Vector3 slotPosition, float duration, Action onComplete = null)
        {
            _slotTween?.Kill();
            _slotTween = null;

            IsReadyToFire = false;

            if (duration <= 0f)
            {
                transform.position = slotPosition;
                transform.localScale = _initialScale;
                IsReadyToFire = true;
                onComplete?.Invoke();
                
                return;
            }

            float scaleDownDuration = duration * 0.35f;
            float scaleUpDuration = duration - scaleDownDuration;

            _slotTween = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.zero, scaleDownDuration).SetEase(Ease.InBack))
                .AppendCallback(() => transform.position = slotPosition)
                .Append(transform.DOScale(_initialScale, scaleUpDuration).SetEase(Ease.OutElastic))
                .OnComplete(() =>
                {
                    _slotTween = null;
                    IsReadyToFire = true;
                    onComplete?.Invoke();
                });
        }

        private void Paint(BlockColor blockColor)
        {
            meshRenderer.sharedMaterial = blockColorSo.GetMaterial(blockColor);
        }
        
        public void OnSpawn()
        {
            if (_initialScale == Vector3.zero)
                _initialScale = transform.localScale;

            transform.localScale = _initialScale;
            IsReadyToFire = false;
        }

        public void OnDespawn()
        {
            _slotTween?.Kill();
            _slotTween = null;
            transform.localScale = _initialScale;
            IsReadyToFire = false;
        }
    }
}
