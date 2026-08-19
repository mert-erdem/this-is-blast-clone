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
    public class TargetBlock : MonoBehaviour, IPoolObject
    {
        [Header("Visual's Components")]
        [SerializeField] private Transform transformVisual;

        [SerializeField] private TextMeshPro textHealth; 
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private BlockColorSo blockColorSo;
        
        public event Action<TargetBlock, float> OnDestroyed;
        
        public bool IsFireable => IsSpawned && !_isMoving && !IsReserved;
        public bool IsReserved { get; set; }

        public bool IsSpawned { get; set; }
        public GameObject GameObject => gameObject;
        
        private BlockColor _color;
        private int _health;
        private Vector2Int _gridPosition;
        
        // Tween Related
        private const float TWEEN_DESTROY_DURATION = 0.2f;
        private Tween _moveTween;
        private Tween _scaleTween;
        private Tween _visualScaleTween;
        private Vector3 _initialScale;
        private Vector3 _initialVisualScale;
        private bool _isMoving;

        // Will be used by ObjectPool
        public void Initialize(TargetBlockData data, Vector2Int gridPosition)
        {
            _moveTween?.Kill();
            _moveTween = null;
            _scaleTween?.Kill();
            _scaleTween = null;
            _visualScaleTween?.Kill();
            _visualScaleTween = null;
            _isMoving = false;
            IsReserved = false;
            transform.localScale = _initialScale;
            
            _color = data.color;
            _health = data.health;
            _gridPosition = gridPosition;
            
            Paint(_color);

            SetHealthText(_health);
            // Tower Block
            SetScale(_health, 0f);
        }

        public void TakeDamage(int amount, float dieTweenDelay = 0f)
        {
            _health -= amount;

            if (_health <= 0)
            {
                OnDestroyed?.Invoke(this, dieTweenDelay);
                return;
            }

            SetHealthText(_health);
            SetScale(_health, 0.15f);
        }

        public Vector2Int GetGridPosition()
        {
            return _gridPosition;
        }

        public void SetGridPosition(Vector2Int gridPosition)
        {
            _gridPosition = gridPosition;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void MoveTo(Vector3 targetPosition, float duration, Action onComplete = null)
        {
            _moveTween?.Kill();

            if (duration <= 0f)
            {
                transform.position = targetPosition;
                _isMoving = false;
                onComplete?.Invoke();
                return;
            }

            _isMoving = true;
            
            _moveTween = transform
                .DOMove(targetPosition, duration)
                .OnComplete(() =>
                {
                    _isMoving = false;
                    _moveTween = null;
                    onComplete?.Invoke();
                });
        }

        public void PlayDieTween(Action onComplete = null, float delay = 0f)
        {
            _scaleTween?.Kill();

            _isMoving = true;
            
            _scaleTween = transform
                .DOScale(Vector3.zero, TWEEN_DESTROY_DURATION)
                .SetDelay(Mathf.Max(0f, delay))
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _isMoving = false;
                    _scaleTween = null;
                    onComplete?.Invoke();
                });
        }

        public BlockColor GetColor()
        {
            return _color;
        }
        
        public int GetHealth()
        {
            return _health;
        }
        
        public void OnSpawn()
        {
            if (_initialScale == Vector3.zero)
                _initialScale = transform.localScale;

            if (_initialVisualScale == Vector3.zero)
                _initialVisualScale = transformVisual.localScale;

            transform.localScale = _initialScale;
            transformVisual.localScale = _initialVisualScale;
        }

        public void OnDespawn()
        {
            _moveTween?.Kill();
            _moveTween = null;
            _scaleTween?.Kill();
            _scaleTween = null;
            _visualScaleTween?.Kill();
            _visualScaleTween = null;
            _isMoving = false;
            IsReserved = false;
            transform.localScale = _initialScale;
            transformVisual.localScale = _initialVisualScale;
        }
        
        private void Paint(BlockColor blockColor)
        {
            meshRenderer.sharedMaterial = blockColorSo.GetMaterial(blockColor);
        }

        /// <summary>
        /// Health based scaling tween for Tower Block.
        /// </summary>
        private void SetScale(int health, float duration)
        {
            float scale = _initialVisualScale.y + (health - 1) * 0.75f;
            
            _visualScaleTween?.Kill();
            _visualScaleTween = transformVisual.DOScaleY(scale, duration);
        }
        
        private void SetHealthText(int health)
        {
            bool shouldShowHealth = health > 1;
            
            textHealth.gameObject.SetActive(shouldShowHealth);

            if (!shouldShowHealth)
                return;

            textHealth.text = health.ToString();
        }
    }
}
