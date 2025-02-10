using UnityEngine;
using UnityEngine.UI;

namespace Universal
{
    public class ColorAnimation : IAnimatedButtonComponent
    {
        [SerializeField] private Image _image;
        [SerializeField] private Color _enabled;
        [SerializeField] private Color _disabled;
        [SerializeField] private Color _selected;
        [SerializeField] private Color _pressed;

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
            _image.color = _pressed;
        }

        public override void ChangeInteractive(bool isInteractive)
        {
            _image.color = isInteractive ? _enabled : _disabled;
        }
    }
}