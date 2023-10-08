using CauldronCodebase;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Universal
{
    public class LinkButton: GrowOnMouseEnter
    {
        public string link = "https://vk.com/theironhearthg";
        
        [Inject] private SoundManager sound;
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (!string.IsNullOrEmpty(link))
            {
                Application.OpenURL(link);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            sound.Play(Sounds.MenuFocus);
        }
    }
}