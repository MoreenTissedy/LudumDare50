using System;
using Buttons;
using UnityEngine;

namespace Universal
{
    public class FlexibleButton : Selectable
    {
        [SerializeField] private IAnimatedButtonComponent[] _animationComponents;
        [SerializeField] private bool _interactive = true;

        public event Action OnClick;
        
        public bool IsInteractive
        {
            get { return _interactive; }
            set {
                    _interactive = value;
                    foreach (var component in _animationComponents)
                    {
                        component?.ChangeInteractive(value);
                    }
                }
        }

        private void Reset()
        {
            _animationComponents = GetComponents<IAnimatedButtonComponent>();
        }

        private void Awake()
        {
            IsInteractive = _interactive;
        }

        public override void Select()
        {
            foreach (var component in _animationComponents)
            {
                component?.Select();
            }
        }

        public override void Unselect()
        {
            foreach (var component in _animationComponents)
            {
                component?.Unselect();
            }
        }

        public void Activate()
        {
           foreach (var component in _animationComponents)
                {
                    component?.Activate();
                }

            OnClick?.Invoke();
        }

        public void RemoveAllSubscriptions()
        {
            OnClick = null;
        }
    }
}