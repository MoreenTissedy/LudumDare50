using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class CloseBookButton : GrowOnMouseEnter
    {
        [Inject] private RecipeBook book;
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            book.CloseBook();
        }
    }
}