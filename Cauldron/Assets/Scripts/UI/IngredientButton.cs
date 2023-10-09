using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class IngredientButton : GrowOnMouseEnter
    {
        [SerializeField] private ScrollTooltip scrollTooltip;
        [SerializeField] private Image image;
        
        private IngredientsData.Ingredient data;
        private bool interactable;

        [Inject] private IngredientsData ingredientsData;
        [Inject] private RecipeBook recipeBook;

        public void Set(Ingredients ingredient)
        {
            data = ingredientsData.Get(ingredient);
            image.enabled = true;
            image.sprite = data.image;
            interactable = true;
            scrollTooltip.Close();
        }

        public void Clear()
        {
            interactable = false;
            image.enabled = false;
            scrollTooltip.Close();
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            base.OnPointerEnter(eventData);
            scrollTooltip.Open(data.friendlyName).Forget();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            base.OnPointerExit(eventData);
            scrollTooltip.Close();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            base.OnPointerClick(eventData);
            recipeBook.ChangeMode(RecipeBook.Mode.Ingredients);
            recipeBook.OpenPage(ingredientsData.IndexOf(data)/2);
        }
    }
}