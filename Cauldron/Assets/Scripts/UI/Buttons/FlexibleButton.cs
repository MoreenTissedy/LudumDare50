using System;
using Buttons;
using UnityEngine;

namespace Universal
{
    public class FlexibleButton : Selectable
    {
        [SerializeField] private AnimatedButtonComponent[] _animationComponents;
        [SerializeField] private bool _interactive = true;

        private bool selected;

        public event Action OnClick;

        public bool IsInteractive
        {
            get { return _interactive; }
            set
            {
                _interactive = value;
                foreach (var component in _animationComponents)
                {
                    component?.ChangeInteractive(value);
                }
            }
        }

        private void OnValidate()
        {
            _animationComponents = GetComponents<AnimatedButtonComponent>();
        }

        private void Awake()
        {
            IsInteractive = _interactive;
        }

        public override void Select()
        {
            selected = true;
            foreach (var component in _animationComponents)
            {
                component?.Select();
            }
        }

        public override void Unselect()
        {
            selected = false;
            foreach (var component in _animationComponents)
            {
                component?.Unselect();
            }
        }

        public override void Activate()
        {
            foreach (var component in _animationComponents)
            {
                component?.Activate();
            }

            OnClick?.Invoke();
        }

        public override bool IsSelected() => selected;

        public void RemoveAllSubscriptions()
        {
            OnClick = null;
        }
    }
}