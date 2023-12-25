using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public abstract class RecipeBookEntryHolder : MonoBehaviour
    {
        [SerializeField] private RecipeBookEntry unlocked;
        [SerializeField] private RecipeBookEntry locked;

        [Inject]
        public void Construct(IngredientsData data)
        {
            Clear();
        }

        public void SetUnlocked(Recipe recipe)
        {
            locked.gameObject.SetActive(false);
            unlocked.gameObject.SetActive(true);
            unlocked.Display(recipe);
        }

        public void SetLocked(Recipe recipe)
        {
            locked.gameObject.SetActive(true);
            unlocked.gameObject.SetActive(false);
            locked.Display(recipe);
        }
        
        public void Clear()
        {
            locked.gameObject.SetActive(false);
            unlocked.gameObject.SetActive(true);
            unlocked.Clear();
        }
    }
    
    public class RecipeBookEntry : MonoBehaviour
    {
        [Header("Recipe hint")]
        public GameObject recipeHintBlock;
        public TMP_Text recipeHintText;
        public RecipeHintsStorage recipeHintsStorage;
        
        [Header("Common")]
        public TMP_Text fullName;
        public TMP_Text description;
        public Image image;
        public IngredientButton ingredient1, ingredient2, ingredient3;
        private Recipe currentRecipe;
        public Recipe CurrentRecipe => currentRecipe;
        
        public void Display(Recipe recipe)
        {
            currentRecipe = recipe;
            image.enabled = true;
            image.sprite = recipe.image;
            DisplayRecipeHint(recipe.potion);
            
            if (fullName)
            {
                fullName.text = recipe.potionName;
            }

            if (description)
            {
                description.text = recipe.description;
            }

            if (ingredient1 && ingredient2 && ingredient3)
            {
                ingredient1.Set(recipe.RecipeIngredients[0]);

                ingredient2.Set(recipe.RecipeIngredients[1]);
                ingredient3.Set(recipe.RecipeIngredients[2]);
            }
        }

        public void Clear()
        {
            currentRecipe = null;
            fullName.text = "";
            description.text = "";
            image.enabled = false;
            ingredient1.Clear();
            ingredient2.Clear();
            ingredient3.Clear();
            recipeHintBlock?.SetActive(false);
        }

        private void DisplayRecipeHint(Potions potion)
        {
            if (!recipeHintBlock)
            {
                return;
            }
            if (recipeHintsStorage.TryGetHint(potion, out var text))
            {
                recipeHintBlock.SetActive(true);
                recipeHintText.text = text;
            }
            else
            {
                recipeHintBlock.SetActive(false);
            }
        }
    }
}