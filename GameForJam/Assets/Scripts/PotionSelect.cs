using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class PotionSelect : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            RecipeBook.instance.SwitchHighlight(GetComponentInParent<RecipeBookEntry>());
        }
    }
}