using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class PotionSelect : GrowOnMouseEnter, IPointerClickHandler
    {
        [Inject] private RecipeBook book;
        [Inject] private SoundManager soundManager;

        public bool clickable = true;
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!clickable || book.isNightBook)
            {
                return;
            }
            base.OnPointerClick(eventData);
            book.SwitchHighlight(GetComponentInParent<RecipeBookEntry>());
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!clickable)
            {
                return;
            }
            base.OnPointerEnter(eventData);
            soundManager.Play(Sounds.BookFocus);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!clickable)
            {
                return;
            }
            base.OnPointerExit(eventData);
        }
    }
}