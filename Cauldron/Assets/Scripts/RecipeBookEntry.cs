using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Selectable = Buttons.Selectable;

namespace CauldronCodebase
{
    public abstract class RecipeBookEntryHolder : Selectable, IOverlayElement
    {
        [SerializeField] private RecipeBookEntry unlocked;
        [SerializeField] private RecipeBookEntry locked;
        [SerializeField] private Image selectionImage;
        [SerializeField] private FlexibleButton potionButton;
        [SerializeField] private Color selectionColor;

        private bool potionUnlocked;
        private bool selectionLock;

        private void Awake()
        {
            Clear();
        }

        public void SetUnlocked(Recipe recipe)
        {
            locked.gameObject.SetActive(false);
            unlocked.gameObject.SetActive(true);
            unlocked.Display(recipe);
            potionUnlocked = true;
        }

        public void SetLocked(Recipe recipe)
        {
            locked.gameObject.SetActive(true);
            unlocked.gameObject.SetActive(false);
            locked.Display(recipe);
            potionUnlocked = false;
        }
        
        public void Clear()
        {
            locked.gameObject.SetActive(false);
            unlocked.gameObject.SetActive(true);
            unlocked.Clear();
        }

        public override void Select()
        {
            if (selectionLock)
            {
                return;
            }

            selectionImage.color = selectionColor;
            if (potionUnlocked)
            {
                potionButton.Select();
            }
        }

        public override void Unselect()
        {
            if (selectionLock)
            {
                return;
            }

            selectionImage.color = Color.white;
            if (potionUnlocked)
            {
                potionButton.Unselect();
            }
        }

        public override void Activate()
        {
            if (potionUnlocked && !selectionLock)
            {
                Unselect();
                potionButton.Activate();
            }
        }

        public void Lock(bool on)
        {
            selectionLock = on;
        }

        public bool IsLocked() => selectionLock;
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
            if (recipeHintBlock)
            {
                recipeHintBlock.SetActive(false);
            }
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