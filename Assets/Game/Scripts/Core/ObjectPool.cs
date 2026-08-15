using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class ObjectPool<T, T1> : MonoBehaviour where T : IPoolObject where T1 : Component
    {
        private static ObjectPool<T, T1> _instance;
        public static ObjectPool<T, T1> Instance => _instance ??= FindFirstObjectByType<ObjectPool<T, T1>>();

        [SerializeField] private T poolObject;
        [SerializeField] private int poolSize;
        [SerializeField] private float objectLifetime = 1f;
        
        private List<T> _pool;
        private List<T> _cache = new();

        public float ObjectLifeTime
        {
            set { objectLifetime = value; }
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(this.transform.parent);
        }

        private void Start()
        {
            _pool = new List<T>();

            for (int i = 0; i < poolSize; i++)
            {
                var instantiatedObject = Instantiate(poolObject.GameObject, transform);
                var objectForPool = instantiatedObject.GetComponent<T>();
                objectForPool.GameObject.SetActive(false);
                _pool.Add(objectForPool);
            }
        }

        public T GetObject(bool setActiveStatus = true)
        {
            for (int i = 0; i < poolSize; i++)
            {
                if (!_pool[i].IsSpawned)
                {
                    _pool[i].IsSpawned = true;
                    _pool[i].GameObject.SetActive(setActiveStatus);
                    _pool[i].OnSpawn();
                    return _pool[i];
                }
            }

            return default(T);
        }

        /// <summary>
        /// Note: This method uses a cache to store the objects that are spawned, 
        /// so its better to use with your own cache.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<T> GetObjects(int count, bool setActiveStatus = true)
        {
            _cache.Clear();

            for (int i = 0; i < poolSize; i++)
            {
                if (!_pool[i].IsSpawned && _cache.Count < count)
                {
                    _pool[i].IsSpawned = true;
                    _pool[i].GameObject.SetActive(setActiveStatus);
                    _pool[i].OnSpawn();
                    _cache.Add(_pool[i]);
                }
            }

            return _cache;
        }

        public void PullObjectBack(T theObject, Action beforePullObjectBack = null)
        {
            if (!_pool.Contains(theObject))
            {
                Destroy(theObject.GameObject);
                return;
            }

            StartCoroutine(PullObjectBackRoutine(theObject, beforePullObjectBack));
        }
        private IEnumerator PullObjectBackRoutine(T theObject, Action beforePullObjectBack = null)
        {
            yield return new WaitForSeconds(objectLifetime);

            beforePullObjectBack?.Invoke();
            theObject.IsSpawned = false;
            theObject.OnDespawn();
            theObject.GameObject.SetActive(false);
        }

        public void PullObjectBackImmediate(T theObject, Action beforePullObjectBack = null)
        {
            if (!_pool.Contains(theObject))
            {
                Destroy(theObject.GameObject);
                return;
            }

            beforePullObjectBack?.Invoke();
            theObject.IsSpawned = false;
            theObject.OnDespawn();
            theObject.GameObject.SetActive(false);
        }

        public void PullGivenObjectsBackImmediate(List<T> objects, Action beforePullObjectBack = null)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                PullObjectBackImmediate(objects[i], beforePullObjectBack);
            }
        }

        public void PullAllObjectsBack(Action beforePullObjectBack = null)
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                PullObjectBackImmediate(_pool[i], beforePullObjectBack);
            }
        }
    }
}