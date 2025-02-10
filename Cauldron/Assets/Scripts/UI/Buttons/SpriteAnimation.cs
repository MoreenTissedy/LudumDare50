using UnityEngine;
using UnityEngine.UI;

namespace Universal
{
    public class SpriteAnimation : IAnimatedButtonComponent
    {
        [SerializeField] private Sprite _selected;
        [SerializeField] private Sprite _enabled;
        [SerializeField] private Sprite _disabled;
        [SerializeField] private Image _image;

        public override void Select()
        {
            _image.sprite = _selected;
        }

        public override void Unselect()
        {
            _image.sprite = _enabled;
        }

        public override void ChangeInteractive(bool isInteractive)
        {
            _image.sprite = isInteractive ? _enabled: _disabled;
        }

        public override void Activate(){}
    }
}