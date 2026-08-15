using System.Collections.Generic;
using Game.Scripts.Core;
using Game.Scripts.Data;
using Game.Scripts.Entities;
using Game.Scripts.ObjectPools;
using UnityEngine;

namespace Game.Scripts.Logic
{
    public class CannonManager : Singleton<CannonManager>
    {
        [SerializeField] private Transform cannonQueuesMidpoint;
        [SerializeField] private float horizontalSpacing = 1f;
        [SerializeField] private float queueSpacing = 1f;

        private Queue<Cannon>[] _cannonQueues; // can be increased via remote config or local JSON
        private readonly List<Cannon> _activeCannons = new();
        
        private ObjectPool<Cannon, CannonPool> _cannonPool;

        public void Initialize(int queueWidth, CannonData[] cannons)
        {
            if (queueWidth <= 0 || cannons == null || cannons.Length == 0)
                return;

            _cannonPool ??= CannonPool.Instance;

            ClearActiveCannons();
            EnsureQueues(queueWidth);
            ClearQueues();

            for (int i = 0; i < cannons.Length; i++)
            {
                int queueIndex = i % queueWidth;
                int queueDepth = i / queueWidth;

                Cannon cannon = _cannonPool.GetObject();

                if (cannon == null)
                {
                    Debug.LogWarning("Cannon pool does not have enough available objects.");
                    return;
                }

                cannon.transform.position = GetCannonPosition(queueIndex, queueDepth, queueWidth);
                cannon.transform.rotation = Quaternion.identity;
                cannon.transform.SetParent(transform, true);

                cannon.Initialize(cannons[i]);

                _cannonQueues[queueIndex].Enqueue(cannon);
                _activeCannons.Add(cannon);
            }
        }

        private Vector3 GetCannonPosition(int queueIndex, int queueDepth, int queueWidth)
        {
            float startX = -((queueWidth - 1) * horizontalSpacing * 0.5f);

            return cannonQueuesMidpoint.position + new Vector3(
                startX + queueIndex * horizontalSpacing,
                0f,
                -queueDepth * queueSpacing);
        }

        private void EnsureQueues(int queueWidth)
        {
            if (_cannonQueues != null && _cannonQueues.Length == queueWidth)
                return;

            _cannonQueues = new Queue<Cannon>[queueWidth];

            for (int i = 0; i < queueWidth; i++)
                _cannonQueues[i] = new Queue<Cannon>();
        }

        private void ClearQueues()
        {
            if (_cannonQueues == null)
                return;

            foreach (Queue<Cannon> queue in _cannonQueues)
                queue.Clear();
        }

        private void ClearActiveCannons()
        {
            if (_cannonPool == null)
                return;

            foreach (Cannon cannon in _activeCannons)
            {
                if (cannon == null)
                    continue;

                _cannonPool.PullObjectBackImmediate(cannon);
            }

            _activeCannons.Clear();
        }
    }
}
