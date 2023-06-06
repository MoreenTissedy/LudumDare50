using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Save;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = System.Random;

namespace CauldronCodebase
{
    public class RecipeBook : Book, IDataPersistence
    {
        [Header("Bookmark")] 
        [SerializeField] private Image bookmark;
        [SerializeField] private Image bookmarkMask;
        [SerializeField] private float bookmarkAnimationSpeed = 0.5f;

        [SerializeField] private Sprite recipeBookmarkSprite;
        [SerializeField] private Sprite foodBookmarkSprite;
        [SerializeField] private Sprite attemptsBookmarkSprite;
        [SerializeField] private Sprite ingredientsBookmarkSprite;

        [Header("Side Bookmarks")] 
        [SerializeField] private Transform recipeSideBookmark;
        [SerializeField] private Transform foodSideBookmark;
        [SerializeField] private Transform attemptsSideBookmark;
        [SerializeField] private Transform ingredientsSideBookmark;
        
        
        [Header("Recipe Book")] 
        
        [SerializeField] protected RecipeBookEntry[] recipeEntries;
        [SerializeField] protected RecipeBookEntry[] foodEntries;
        [SerializeField] protected AttemptEntry[] attemptEntries;
        [SerializeField] protected IngredientInTheBook[] ingredientsEntries;
        [SerializeField] protected IngredientsData ingredientsData;
        public List<Recipe> magicalRecipes;
        public List<Recipe> herbalRecipes;
        public List<WrongPotion> wrongPotions;
        [SerializeField] protected Text prevPageNum, nextPageNum;
        [SerializeField] private GameObject recipesDisplay, foodDisplay, attemptsDisplay, ingredientsDisplay;
        public event Action<Recipe> OnSelectRecipe;
        
        private TooltipManager tooltipManager;
        private RecipeProvider recipeProvider;
        private Cauldron cauldron;

        public static int MAX_COMBINATIONS_COUNT = 120;

        private Mode currentMode;
        public enum Mode
        {
            Magical,
            Herbal,
            Attempts,
            Ingredients
        }

        [ContextMenu("Find Entries")]
        void FindEntries()
        {
            attemptEntries = attemptsDisplay.GetComponentsInChildren<AttemptEntry>();
            recipeEntries = recipesDisplay.GetComponentsInChildren<RecipeBookEntry>();
            foodEntries = foodDisplay.GetComponentsInChildren<RecipeBookEntry>();
        }
        [Inject]
        private void Construct(DataPersistenceManager dataPersistenceManager,
                                TooltipManager tooltipManager,
                                RecipeProvider recipeProvider,
                                Cauldron cauldron)
        {
            dataPersistenceManager.AddToDataPersistenceObjList(this);
            this.tooltipManager = tooltipManager;
            this.recipeProvider = recipeProvider;
            this.cauldron = cauldron;
        }

        private void Start()
        {
            LoadRecipes();
        }

        public override void OpenBook()
        {
            base.OpenBook();
            ChangeMode(Mode.Magical);
        }

        private void LoadRecipes()
        {
            foreach (Recipe recipe in recipeProvider.LoadRecipes())
            {
                if (recipe.magical && !magicalRecipes.Contains(recipe))
                {
                    magicalRecipes.Add(recipe);
                }
                else if (!recipe.magical && !herbalRecipes.Contains(recipe))
                {
                    herbalRecipes.Add(recipe);
                }
            }
        }

        public void RecordAttempt(WrongPotion mix)
        {
            if (wrongPotions is null)
            {
                wrongPotions = new List<WrongPotion>(10);
            }

            if (!wrongPotions.Any(wrongRecipe => wrongRecipe.IngredientsList.All(mix.IngredientsList.Contains)))
            {
                wrongPotions.Add(mix);
            }
        }

        public bool IsRecipeInBook(Recipe recipe)
        {
            if (recipe.magical)
            {
                return magicalRecipes.Contains(recipe);
            }
            else
            {
                return herbalRecipes.Contains(recipe);
            }
        }

        public void RecordRecipe(Recipe recipe)
        {
            if (recipe.magical)
            {
                magicalRecipes.Add(recipe);
            }
            else
            {
                if (herbalRecipes is null)
                {
                    herbalRecipes = new List<Recipe>(10);
                }
                herbalRecipes.Add(recipe);
            }
            recipeProvider.SaveRecipes(magicalRecipes.Union(herbalRecipes));
        }

        public void ChangeMode(Mode newMode)
        {

            switch (newMode)
            {
                case Mode.Magical:
                    CloseAllPages();
                    recipesDisplay.SetActive(true);
                    break;
                case Mode.Herbal:
                    CloseAllPages();
                    foodDisplay.SetActive(true);
                    break;
                case Mode.Attempts:
                    CloseAllPages();
                    attemptsDisplay.SetActive(true);
                    break;
                case Mode.Ingredients:
                    CloseAllPages();
                    ingredientsDisplay.SetActive(true);
                    break;
            }
            ChangeBookmarksOrder(newMode);
            
            currentMode = newMode;
            currentPage = 0;
            InitTotalPages();
            UpdatePage();
            UpdateBookButtons();
            
        }

        private void ChangeBookmarksOrder(Mode newMode)
        {
            Transform GetTransformFromMode(Mode mode)
            {
                switch (mode)
                {
                    case Mode.Magical:
                        return recipeSideBookmark;

                    case Mode.Herbal:
                        return foodSideBookmark;

                    case Mode.Attempts:
                        return attemptsSideBookmark;

                    case Mode.Ingredients:
                        return ingredientsSideBookmark;
                    
                    default: return recipeSideBookmark;
                }
            }

            void SetBookmarkSprite()
            {
                switch (newMode)
                {
                    case Mode.Magical:
                        bookmark.sprite = recipeBookmarkSprite;
                        break;
                    case Mode.Herbal:
                        bookmark.sprite = foodBookmarkSprite;
                        break;
                    case Mode.Attempts:
                        bookmark.sprite = attemptsBookmarkSprite;
                        break;
                    case Mode.Ingredients:
                        bookmark.sprite = ingredientsBookmarkSprite;
                        break;
                }
            }

            var oldTransform = GetTransformFromMode(currentMode);
            var newTransform = GetTransformFromMode(newMode);
            
            int oldBookmarkOrder = oldTransform.GetSiblingIndex();
            int newBookmarkOrder = newTransform.GetSiblingIndex();
            
            oldTransform.SetSiblingIndex(newBookmarkOrder);
            newTransform.SetSiblingIndex(oldBookmarkOrder);
            
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(bookmarkMask.DOFillAmount(0, bookmarkAnimationSpeed))
                      .AppendCallback(SetBookmarkSprite)
                      .Append(bookmarkMask.DOFillAmount(1, bookmarkAnimationSpeed));

        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space))
                ToggleBook();
        }

        protected override void InitTotalPages()
        {
            switch (currentMode)
            {
                case Mode.Magical:
                    totalPages = Mathf.CeilToInt((float)magicalRecipes.Count / recipeEntries.Length);
                    break;
                case Mode.Herbal:
                    if (herbalRecipes != null)
                        totalPages = Mathf.CeilToInt((float) herbalRecipes.Count / foodEntries.Length);
                    else totalPages = 1;
                    break;
                case Mode.Attempts:
                    if (wrongPotions != null) totalPages = Mathf.CeilToInt((float) wrongPotions.Count / attemptEntries.Length);
                    else totalPages = 1;
                    break;
                case Mode.Ingredients:
                    totalPages = Mathf.CeilToInt((float)ingredientsData.book.Length / ingredientsEntries.Length);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void UpdatePage()
        {
            void DisplaySet(List<Recipe> set)
            {
                RecipeBookEntry[] entries = new RecipeBookEntry[] { };
                switch (currentMode)
                {
                    case Mode.Magical:
                        entries = recipeEntries;
                        break;
                    case Mode.Herbal:
                        entries = foodEntries;
                        break;
                }
                
                if (set is null || set.Count == 0)
                {
                    foreach (var entry in entries)
                    {
                        entry.Clear();
                    }
                    return;   
                }
                for (int i = 0; i < entries.Length; i++)
                {
                    int num = currentPage * entries.Length + i;
                    if (num < set.Count)
                    {
                        entries[i].Display(set[num]);
                    }
                    else
                    {
                        entries[i].Clear();
                    }
                }
            }

            switch (currentMode)
            {
                case Mode.Magical:
                    DisplaySet(magicalRecipes);
                    break;
                case Mode.Herbal:
                    DisplaySet(herbalRecipes);
                    break;
                case Mode.Attempts:
                    DisplayAttempts();
                    break;
                case Mode.Ingredients:
                    DisplayIngredients();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            nextPageNum.text = (currentPage *2+2).ToString();
            prevPageNum.text = (currentPage*2+1).ToString();
        }

        private void DisplayIngredients()
        {
            for (int i = 0; i < ingredientsEntries.Length; i++)
            {
                int num = currentPage * ingredientsEntries.Length + i;
                
                if (ingredientsData.book[num].TopImage == null)
                {
                    ingredientsEntries[i].topImage.gameObject.SetActive(false);
                }
                else
                {
                    ingredientsEntries[i].topImage.gameObject.SetActive(true);
                    ingredientsEntries[i].topImage.sprite = ingredientsData.book[num].TopImage;
                }
                
                if (ingredientsData.book[num].BottomImage == null)
                {
                    ingredientsEntries[i].bottomImage.gameObject.SetActive(false);
                }
                else
                {
                    ingredientsEntries[i].bottomImage.gameObject.SetActive(true);
                    ingredientsEntries[i].bottomImage.sprite = ingredientsData.book[num].BottomImage;
                }
                
                if (ingredientsData.book[num].TextInABook == null)
                {
                    ingredientsEntries[i].text.gameObject.SetActive(false);
                }
                else
                {
                    ingredientsEntries[i].text.gameObject.SetActive(true);
                    ingredientsEntries[i].text.text = ingredientsData.book[num].TextInABook;
                }
                
                //ingredientsEntries[i].RebuildLayout();
            }
        }

        private void DisplayAttempts()
        {
            if (wrongPotions is null || wrongPotions.Count == 0)
                return;
            for (int i = 0; i < attemptEntries.Length; i++)
            {
                int num = currentPage * recipeEntries.Length + i;
                if (num < wrongPotions.Count)
                {
                    attemptEntries[i].Display(wrongPotions[num].IngredientsList.ToArray());
                }
                else
                {
                    attemptEntries[i].Clear();
                }
            }
        }

        private void CloseAllPages()
        {
            recipesDisplay.SetActive(false);
            foodDisplay.SetActive(false);
            attemptsDisplay.SetActive(false);
            ingredientsDisplay.SetActive(false);
        }
        public void SwitchHighlight(RecipeBookEntry recipeBookEntry)
        {
            if (cauldron.Mix.Count != 0)
            {
                foreach (var ingredient in cauldron.Mix)
                {
                    if (recipeBookEntry.CurrentRecipe.RecipeIngredients.Contains(ingredient) == false)
                    {
                        TryHighlightIncorrectRecipe();
                        return;
                    }
                }
            }
            
            CloseBook();
            tooltipManager.HighlightRecipe(recipeBookEntry.CurrentRecipe);
            OnSelectRecipe?.Invoke(recipeBookEntry.CurrentRecipe);
        }

        private void TryHighlightIncorrectRecipe()
        {
            Debug.LogWarning("The ingredients in the cauldron do not match this recipe"); 
        }
        
        public Ingredients[] GenerateRandomRecipe()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
        
            var ingredients = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>().ToList();

            return ingredients.OrderBy(x => rnd.Next()).Take(3).ToArray();

        }
    
        public bool CheckRecipeIsOpen(Ingredients[] recipe)
        {
            
            //Check magic
            if (magicalRecipes.Any(magicalRecipe => magicalRecipe.RecipeIngredients.All(recipe.Contains)))
            {
                return true;
            }
        
            //Check food
            if (herbalRecipes.Any(foodRecipe => foodRecipe.RecipeIngredients.All(recipe.Contains)))
            {
                return true;
            }
            
            //Check attempts
            if(wrongPotions.Count != 0 && wrongPotions.Any(wrongRecipe => wrongRecipe.IngredientsList.All(recipe.Contains)))
            {
                return true;
            }

            return false;
        }

        public void LoadData(GameData data, bool newGame)
        {
            wrongPotions = data.AttemptsRecipes;
            foreach (var potion in wrongPotions)
            {
                potion.RestoreIngredients();
            }
            
        }

        public void SaveData(ref GameData data)
        {
            data.AttemptsRecipes = wrongPotions;
        }
    }
}