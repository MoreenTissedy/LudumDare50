using System;
using UnityEngine;

namespace Universal
{
    public class AnimatedButtonContainer : MonoBehaviour
    {
        [SerializeField] private IAnimatedButtonComponent[] _animationComponents;
        private bool _interactive = true;

        [SerializeField] public event Action OnClick;
        
        public bool IsInteractive
        {
            get { return _interactive; }
            set {
                    _interactive = value;
                    foreach (var component in _animationComponents)
                    {
                        component.ChangeInteractive(value);
                    }
                }
        }

        private void OnValidate()
        {
            _animationComponents = GetComponents<IAnimatedButtonComponent>();
        }

        private void Awake()
        {
            IsInteractive = _interactive;
        }

        public void Selected()
        {
            foreach (var component in _animationComponents)
            {
                component.Select();
            }
        }

        public void Unselected()
        {
            foreach (var component in _animationComponents)
            {
                component.Unselect();
            }
        }

        public void Activate()
        {
            foreach (var component in _animationComponents)
            {
                component.Activate();
            }
            OnClick?.Invoke();
        }

        public void RemoveAllSubscriptions()
        {
            OnClick = null;
        }
    }
}