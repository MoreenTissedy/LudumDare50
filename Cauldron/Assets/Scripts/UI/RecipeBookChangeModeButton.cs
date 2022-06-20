using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookChangeModeButton : MonoBehaviour, IPointerClickHandler
    {
        public RecipeBook.Mode mode;
        [Inject]
        private RecipeBook recipeBook;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            recipeBook.ChangeMode(mode);
        }
    }
}