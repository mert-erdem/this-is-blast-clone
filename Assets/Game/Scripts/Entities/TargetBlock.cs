using System;
using DG.Tweening;
using Game.ScriptableObjects;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Enums;
using UnityEngine;

namespace Game.Scripts.Entities
{
    public class TargetBlock : MonoBehaviour, IPoolObject
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BlockColorSo blockColorSo;
        
        public event Action<TargetBlock> OnDestroyed;
        
        public bool IsMoving { get; private set; }

        public bool IsFireable => IsSpawned && !IsMoving;

        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        private BlockColor _color;
        private int _health;
        private Vector2Int _gridPosition;
        private Tween _moveTween;

        // Will be used by ObjectPool
        public void Initialize(TargetBlockData data, Vector2Int gridPosition)
        {
            _moveTween?.Kill();
            _moveTween = null;
            IsMoving = false;
            
            _color = data.color;
            _health = data.health;
            _gridPosition = gridPosition;
            
            Paint(_color);
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;

            if (_health <= 0)
            {
                OnDestroyed?.Invoke(this);
            }
        }

        public Vector2Int GetGridPosition()
        {
            return _gridPosition;
        }

        public void SetGridPosition(Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public void MoveTo(Vector3 targetPosition, float duration, Action onComplete = null)
        {
            _moveTween?.Kill();

            if (duration <= 0f)
            {
                transform.position = targetPosition;
                IsMoving = false;
                onComplete?.Invoke();
                return;
            }

            IsMoving = true;
            
            _moveTween = transform
                .DOMove(targetPosition, duration)
                .OnComplete(() =>
                {
                    IsMoving = false;
                    _moveTween = null;
                    onComplete?.Invoke();
                });
        }

        public BlockColor GetColor()
        {
            return _color;
        }
        
        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            _moveTween?.Kill();
            _moveTween = null;
            IsMoving = false;
        }
        
        private void Die(){}

        private void Paint(BlockColor blockColor)
        {
            meshRenderer.sharedMaterial = blockColorSo.GetMaterial(blockColor);
        }
    }
}
