using UnityEngine;
using UnityEngine.EventSystems;
using Universal;
using Zenject;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class IngredientButton : MonoBehaviour
    {
        [SerializeField] private ScrollTooltip scrollTooltip;
        [SerializeField] private Image image;
        [SerializeField] private Material material;
        [SerializeField] private FlexibleButton button;
        
        private IngredientsData.Ingredient data;

        [Inject] private IngredientsData ingredientsData;
        [Inject] private RecipeBook recipeBook;
        [Inject] private GameDataHandler gameData;

        public async void Set(Ingredients ingredient)
        {
            data = ingredientsData.Get(ingredient);
            image.enabled = true;
            image.sprite = data.image;
            image.material = gameData.ingredientsFreezed.Contains(ingredient) ? material : null;
            button.IsInteractive = false;
            scrollTooltip.Close();
            await scrollTooltip.SetText(data.friendlyName);
            button.IsInteractive = true;
        }

        private void Awake()
        {
            button.OnClick += OnClick;
        }

        public void Clear()
        {
            button.IsInteractive = false;
            image.enabled = false;
            scrollTooltip.Close();
        }

        public void OnClick()
        {
            recipeBook.ChangeMode(RecipeBook.Mode.Ingredients);
            recipeBook.OpenPage(ingredientsData.IndexOf(data)/2);
        }

        private void OnDestroy()
        {
            button.OnClick -= OnClick;
        }
    }
}