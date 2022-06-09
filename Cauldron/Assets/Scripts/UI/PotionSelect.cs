using UnityEngine;
using UnityEngine.EventSystems;

namespace CauldronCodebase
{
    public class PotionSelect : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            RecipeBook.instance.SwitchHighlight(GetComponentInParent<RecipeBookEntry>());
        }
    }
}