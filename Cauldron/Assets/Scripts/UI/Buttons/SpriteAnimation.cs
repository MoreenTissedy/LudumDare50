using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Universal
{
    public class SpriteAnimation : IAnimatedButtonComponent
    {
        [SerializeField] private Sprite _enabled;
        [SerializeField] private Sprite _disabled;
        [SerializeField] private Sprite _selected;
        [SerializeField] private Sprite _activate;
        [Space]
        [SerializeField] private Image _image;
        [SerializeField] private float _duration = 0.2f;

        private Sprite _default;
        private Coroutine _coroutine; 

        private void Awake()
        {
            _default = _enabled ?? _image.sprite;
        }

        private void OnEnable()
        {
            _image.sprite = _default;
        }

        public override void Select()
        {
            _image.sprite = _selected ?? _default;
        }

        public override void Unselect()
        {
            _image.sprite = _default;
        }

        public override void ChangeInteractive(bool isInteractive)
        {
            _image.sprite = isInteractive ? _enabled : _disabled;
            _default = _image.sprite;
        }

        public override void Activate()
        {
            if(_activate == null) return;

            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(SetActivateSprite());
        }

        private IEnumerator SetActivateSprite()
        {
            _image.sprite = _activate;

            yield return new WaitForSeconds(_duration);
            _image.sprite = _default;
        }
    }
}