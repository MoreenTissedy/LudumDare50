using UnityEngine;
using UnityEngine.EventSystems;
using Universal;

namespace CauldronCodebase
{
    public class CloseBookButton : GrowOnMouseEnter
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            RecipeBook.instance.CloseBook();
        }
    }
}