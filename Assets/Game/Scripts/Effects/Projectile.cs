using System;
using DG.Tweening;
using Game.ScriptableObjects;
using Game.Scripts.Core;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class Projectile : MonoBehaviour, IPoolObject
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BlockColorSo blockColorSo;
        
        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        // Tween Related
        private const float MOVE_TWEEN_DURATION = 0.5f;
        private Tween _moveTween;

        public void Initialize(Vector3 targetPosition, BlockColor color)
        {
            Paint(color);
            PlayMoveTween(targetPosition, MOVE_TWEEN_DURATION);
        }

        private void PlayMoveTween(Vector3 targetPosition, float duration, Action onComplete = null)
        {
            _moveTween?.Kill();
            _moveTween = null;
            
            _moveTween = transform
                .DOMove(targetPosition, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _moveTween = null;
                    onComplete?.Invoke();
                });
        }

        private void Paint(BlockColor color)
        {
            meshRenderer.sharedMaterial = blockColorSo.GetMaterial(color);
        }
        
        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }
    }
}
