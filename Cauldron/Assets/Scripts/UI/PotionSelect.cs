using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CauldronCodebase
{
    public class PotionSelect : MonoBehaviour, IPointerClickHandler
    {
        [Inject]
        private RecipeBook book;
        public void OnPointerClick(PointerEventData eventData)
        {
            book.SwitchHighlight(GetComponentInParent<RecipeBookEntry>());
        }
    }
}