using System;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class Panel : MonoBehaviour
    {
        public bool IsVisible { get; private set; }

        private Action onPop;

        private void Awake()
        {
            IsVisible = gameObject.activeSelf;
        }

        public void Push(Action onPop = null)
        {
            if (IsVisible) return;
            
            this.onPop = onPop;
            gameObject.SetActive(true);
            IsVisible = true;
        }

        public void Pop()
        {
            if (!IsVisible) return;
            
            onPop?.Invoke();
            gameObject.SetActive(false);
            IsVisible = false;
        }
    }
}