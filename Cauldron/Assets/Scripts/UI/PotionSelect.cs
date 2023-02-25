using FMOD;
using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class PotionSelect : GrowOnMouseEnter, IPointerClickHandler
    {
        [Inject] private RecipeBook book;
        [Inject] private SoundManager soundManager;
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            book.SwitchHighlight(GetComponentInParent<RecipeBookEntry>());
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            soundManager.Play(Sounds.BookFocus);
        }
    }
}