using System.Collections.Generic;
using EasyLoc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class AttemptEntry : MonoBehaviour
    {
        [Inject]
        private IngredientsData data;
        [Inject] 
        private RecipeBook recipeBook;

        [SerializeField] private Image ingredient1Image, ingredient2Image, ingredient3Image;
        [SerializeField] private Image negativeResultImage;
        [SerializeField] private Image unknownResultImage;
        [SerializeField] private Image potionResultImage;
        [SerializeField] private AnimatedButton button;
        [SerializeField] private Material lockedMaterial;
        [SerializeField] private TextMeshProUGUI titleText;
        [Localize] public string failure;
        [Localize] public string notTried;

        private List<Ingredients> ingredients = new List<Ingredients>();

        private void Awake()
        {
            Clear();
        }

        private void OnEnable()
        {
            button.OnClick += () => recipeBook.SwitchHighlight(ingredients).Forget();
        }

        private void OnDisable()
        {
            button.OnClick -= () => recipeBook.SwitchHighlight(ingredients).Forget();
        }

        public void DisplayFailure(Ingredients[] attempt)
        {
            if (attempt.Length != 3)
            {
                return;
            }
            
            ingredient1Image.sprite = data.Get(attempt[0]).image;
            ingredient2Image.sprite = data.Get(attempt[1]).image;
            ingredient3Image.sprite = data.Get(attempt[2]).image;
            SetIngredients(attempt);
            titleText.text = failure;
            negativeResultImage.gameObject.SetActive(true);
            potionResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(false);
            SetLockedMaterial(true);
            gameObject.SetActive(true);
        }

        public void DisplayPotion(Ingredients[] attempt, Recipe recipe)
        {
            if (attempt.Length != 3)
            {
                return;
            }
            
            ingredient1Image.sprite = data.Get(attempt[0]).image;
            ingredient2Image.sprite = data.Get(attempt[1]).image;
            ingredient3Image.sprite = data.Get(attempt[2]).image;
            SetIngredients(attempt);
            titleText.text = recipe.potionName;
            potionResultImage.sprite = recipe.image;
            potionResultImage.gameObject.SetActive(true);
            negativeResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(false);
            SetLockedMaterial(true);
            gameObject.SetActive(true);
        }

        public void DisplayNotTried(Ingredients[] attempt)
        {
            if (attempt.Length != 3)
            {
                return;
            }
            ingredient1Image.sprite = data.Get(attempt[0]).image;
            ingredient2Image.sprite = data.Get(attempt[1]).image;
            ingredient3Image.sprite = data.Get(attempt[2]).image;
            SetIngredients(attempt);
            titleText.text = notTried;
            negativeResultImage.gameObject.SetActive(false);
            potionResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(true);
            SetLockedMaterial(false);
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            ingredients.Clear();
            gameObject.SetActive(false);
            negativeResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(false);
        }

        private void SetLockedMaterial(bool on)
        {
            ingredient1Image.material = on ? lockedMaterial : null;
            ingredient2Image.material = on ? lockedMaterial : null;
            ingredient3Image.material = on ? lockedMaterial : null;
        }

        private void SetIngredients(Ingredients[] attempt)
        {
            ingredients = new List<Ingredients>()
            {
                data.Get(attempt[0]).type,
                data.Get(attempt[1]).type,
                data.Get(attempt[2]).type
            };
        }
    }
}