using UnityEngine;
using UnityEngine.UI;

namespace Universal
{
    public class ColorAnimation : AnimatedButtonComponent
    {
        [SerializeField] private Image _image;
        [Space]
        [SerializeField] private Color _enabled;
        [SerializeField] private Color _disabled;
        [SerializeField] private Color _selected;
        [SerializeField] private Color _pressed;

        private Color _default;
        
        private void Awake()
        {
            _default = _enabled;
        }

        private void OnEnable()
        {
            _image.color = _default;
        }

        public override void Select()
        {
            _image.color = _selected;
        }

        public override void Unselect()
        {
            _image.color = _enabled;
        }

        public override void Activate()
        {
            //_image.color = _pressed;
        }

        public override void ChangeInteractive(bool isInteractive)
        {
            _image.color = isInteractive ? _enabled : _disabled;
        }
    }
}