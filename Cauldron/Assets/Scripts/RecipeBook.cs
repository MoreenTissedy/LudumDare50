using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Save;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        public List<Recipe> allMagicalRecipes;
        public List<Recipe> allHerbalRecipes;
        [SerializeField] private List<Recipe> unlockedRecipes;
        public List<WrongPotion> wrongPotions;
        [SerializeField] protected Text prevPageNum, nextPageNum;
        [SerializeField] private GameObject recipesDisplay, foodDisplay, attemptsDisplay, ingredientsDisplay;
        public event Action<Recipe> OnSelectRecipe;
        public event Action OnOpenBook;
        
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
            OnOpenBook?.Invoke();
        }

        private void LoadRecipes()
        {
            List<Recipe> loadRecipes = recipeProvider.LoadRecipes().ToList();
            if (loadRecipes.Count > 0)
            {
                unlockedRecipes = loadRecipes.ToList();
            }
            
            foreach (Recipe recipe in recipeProvider.allRecipes)
            {
                if (recipe.magical)
                {
                    allMagicalRecipes.Add(recipe);
                }
                else allHerbalRecipes.Add(recipe);
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
            return unlockedRecipes.Contains(recipe);
        }

        public void RecordRecipe(Recipe recipe)
        {
            unlockedRecipes.Add(recipe);
            recipeProvider.SaveRecipes(unlockedRecipes);
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

        private void SortPotions()
        {
            List<Recipe> lockedMagicalRecipes = allMagicalRecipes.Except(unlockedRecipes).ToList();
            allMagicalRecipes.Clear();
            allMagicalRecipes = unlockedRecipes.Where(element => element.magical).ToList();
            allMagicalRecipes.AddRange(lockedMagicalRecipes);

            List<Recipe> lockedHerbalRecipes = allHerbalRecipes.Except(unlockedRecipes).ToList();
            allHerbalRecipes.Clear();
            allHerbalRecipes = unlockedRecipes.Where(element => !element.magical).ToList();
            allHerbalRecipes.AddRange(lockedHerbalRecipes);

        }

        protected override void InitTotalPages()
        {
            SortPotions();
            switch (currentMode)
            {
                case Mode.Magical:
                    totalPages = Mathf.CeilToInt((float)allMagicalRecipes.Count / recipeEntries.Length);
                    break;
                case Mode.Herbal:
                    totalPages = Mathf.CeilToInt((float) allHerbalRecipes.Count / foodEntries.Length);
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
                        if(unlockedRecipes.Contains(set[num]))
                        {
                            entries[i].Display(set[num]);
                        }
                        else
                        {
                            entries[i].DisplayLocked(set[num]);
                        }
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
                    DisplaySet(allMagicalRecipes);
                    break;
                case Mode.Herbal:
                    DisplaySet(allHerbalRecipes);
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

        public bool IsIngredientSetKnown(Ingredients[] recipe)
        {
            //Check potions
            if (unlockedRecipes.Any(magicalRecipe => magicalRecipe.RecipeIngredients.All(recipe.Contains)))
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