using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookChangeModeButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public RecipeBook.Mode mode;
        public ScrollTooltip hint;
        
        [Inject]
        private RecipeBook recipeBook;

        public void OnPointerClick(PointerEventData eventData)
        {
            recipeBook.ChangeMode(mode);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hint.Open();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hint.Close();
        }
    }
}