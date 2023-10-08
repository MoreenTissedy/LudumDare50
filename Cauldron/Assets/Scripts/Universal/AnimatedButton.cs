using System;
using CauldronCodebase;
using UnityEngine.EventSystems;
using Zenject;

namespace Universal
{
    public class AnimatedButton: GrowOnMouseEnter
    {
        public event Action OnClick;
        
        [Inject] private SoundManager sound;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            sound.Play(Sounds.MenuFocus);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            sound.Play(Sounds.MenuClick);
            OnClick?.Invoke();
        }
    }
}