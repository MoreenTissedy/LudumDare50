using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Universal;
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
        
        [Header("Ending map")] 
        [SerializeField] private GameObject endingBookmarkBlock;
        [SerializeField] private AnimatedButton endingBookmarkButton;
        [SerializeField] private Transform endingRoot;
        private EndingScreen endingScreen;
        public SkinSO culinarySkin;

        [Header("Recipe Book")]
        [SerializeField] protected RecipeBookEntryHolder[] recipeEntries;
        [SerializeField] protected RecipeBookEntryHolder[] foodEntries;
        [SerializeField] protected IngredientInTheBook[] ingredientsEntries;
        [SerializeField] protected IngredientsData ingredientsData;

        public List<Recipe> allMagicalRecipes;
        public List<Recipe> allHerbalRecipes;
        [SerializeField] private List<Recipe> unlockedRecipes;
        [SerializeField] private ExperimentController experimentController;
        [SerializeField] private WrongRecipeProvider wrongRecipeProvider;
        public List<Recipe> LockedRecipes { get; private set; }
        public List<Recipe> UnlockedRecipes => unlockedRecipes;
        [SerializeField] protected Text prevPageNum, nextPageNum;
        [SerializeField] private GameObject recipesDisplay, foodDisplay, attemptsDisplay, ingredientsDisplay;

        private const float TargetPercentEnoughRecipesUnlocked = 0.8f;

        public event Action<Recipe> OnSelectRecipe;
        public event Action OnSelectIncorrectRecipe;
        public event Action OnOpenBook;
        public event Action OnUnlockAutoCooking;

        public RecipeBookButton hudButton;
        public bool isNightBook = false;
        
        private TooltipManager tooltipManager;
        private RecipeProvider recipeProvider;
        private Cauldron cauldron;
        private IAchievementManager achievements;
        private EndingsProvider endingsProvider;
        private PlayerProgressProvider progressProvider;
        private GameDataHandler gameData;

        public static int MAX_COMBINATIONS_COUNT = 120;

        private Mode currentMode;
        public Mode CurrentMode => currentMode;
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
            recipeEntries = recipesDisplay.GetComponentsInChildren<RecipeEntryMagical>();
            foodEntries = foodDisplay.GetComponentsInChildren<RecipeEntryCommon>();
        }
        [Inject]
        private void Construct(DataPersistenceManager dataPersistenceManager,
                                TooltipManager tooltipManager,
                                RecipeProvider recipeProvider,
                                Cauldron cauldron, 
                                IAchievementManager achievements, 
                                EndingsProvider endingsProvider,
                                PlayerProgressProvider progressProvider,
                                GameDataHandler gameData)
        {
            dataPersistenceManager.AddToDataPersistenceObjList(this);
            this.achievements = achievements;
            this.tooltipManager = tooltipManager;
            this.recipeProvider = recipeProvider;
            this.cauldron = cauldron;
            this.endingsProvider = endingsProvider;
            this.progressProvider = progressProvider;
            this.gameData = gameData;
        }

        private void Start()
        {
            InitEndingsMap();
            LoadRecipes();
            experimentController.OnContentChanged += () =>
            {
                InitTotalPages();
                OpenPage(0);
            };
        }

        private void InitEndingsMap()
        {
            bool reachedFirstEnding = endingsProvider.GetUnlockedEndingsCount() > 0;
            endingBookmarkBlock.SetActive(reachedFirstEnding);
            if (reachedFirstEnding)
            {
                endingBookmarkButton.OnClick += OpenEndingMap;
            }
        }

        private void OpenEndingMap()
        {
            if (!endingScreen)
            {
                var asset = Resources.Load<EndingScreen>(ResourceIdents.EndingScreen);
                endingScreen = Instantiate(asset, endingRoot);
            }
            if (endingScreen == null)
            {
                return;
            }
            endingScreen.Open(inBook: true);
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

            LockedRecipes = new List<Recipe>(recipeProvider.allRecipes.Except(unlockedRecipes));
        }

        public bool IsRecipeInBook(Recipe recipe)
        {
            return unlockedRecipes.Contains(recipe);
        }

        
        public void CheckExperimentsCompletion()
        {
            if (unlockedRecipes.Count + experimentController.wrongPotions.Count == MAX_COMBINATIONS_COUNT)
            {
                achievements.TryUnlock(AchievIdents.EXPERIMENTS_ALL);
            }
        }

        public void RecordRecipe(Recipe recipe)
        {
            unlockedRecipes.Add(recipe);
            LockedRecipes.Remove(recipe);
            recipeProvider.SaveRecipes(unlockedRecipes);

            if (recipe.magical)
            {
                achievements.TryUnlock(AchievIdents.FIRST_UNLOCK);
                if (AllMagicalRecipesUnlocked())
                {
                    achievements.TryUnlock(AchievIdents.MAGIC_ALL);
                }
            }
            else
            {
                if (unlockedRecipes.Count(x => !x.magical) == allHerbalRecipes.Count)
                {
                    achievements.TryUnlock(AchievIdents.FOOD_ALL);
                }
            }
            
            TryUnlockAutoCooking();
        }

        private void TryUnlockAutoCooking()
        {
            int targetPercent = (int) ((allMagicalRecipes.Count + allHerbalRecipes.Count) * TargetPercentEnoughRecipesUnlocked);
            if (unlockedRecipes.Count < targetPercent || progressProvider.IsAutoCookingUnlocked)
            {
                return;
            }

            progressProvider.SaveAutoCookingUnlocked();
            OnUnlockAutoCooking?.Invoke();
        }

        public bool AllMagicalRecipesUnlocked()
        {
            return unlockedRecipes.Count(x => x.magical) == allMagicalRecipes.Count;
        }
        
        public bool AllHerbalRecipesUnlocked(out SkinSO skin)
        {
            skin = culinarySkin;
            return unlockedRecipes.Count(x => !x.magical) == allHerbalRecipes.Count;
        }

        public void CheatUnlockAutoCooking()
        {
            progressProvider.SaveAutoCookingUnlocked();
            OnUnlockAutoCooking?.Invoke();
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
            switch (currentMode)
            {
                case Mode.Magical:
                    
                    SortPotions();
                    totalPages = Mathf.CeilToInt((float)allMagicalRecipes.Count / recipeEntries.Length);
                    break;
                case Mode.Herbal:
                    
                    SortPotions();
                    totalPages = Mathf.CeilToInt((float) allHerbalRecipes.Count / foodEntries.Length);
                    break;
                case Mode.Attempts:
                    totalPages = experimentController.TotalPages;
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
                RecipeBookEntryHolder[] entries = { };
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
                            entries[i].SetUnlocked(set[num]);
                        }
                        else
                        {
                            entries[i].SetLocked(set[num]);
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
                    experimentController.UpdateTab(currentPage);
                    break;
                case Mode.Ingredients:
                    DisplayIngredients();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            nextPageNum.text = (currentPage * 2 + 2).ToString();
            prevPageNum.text = (currentPage * 2 + 1).ToString();
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
            tooltipManager.DisableAllHighlights();
            
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

            foreach (var ingredient in recipeBookEntry.CurrentRecipe.RecipeIngredients)
            {
                if (gameData.ingredientsFreezed.Contains(ingredient) && !cauldron.Mix.Contains(ingredient))
                {
                    return;
                }
            }
            
            CloseBook();

            if (cauldron.potionPopup.IsEnable)
            {
                return;
            }
            
            tooltipManager.HighlightRecipe(recipeBookEntry.CurrentRecipe);
            OnSelectRecipe?.Invoke(recipeBookEntry.CurrentRecipe);

            if (PlayerPrefs.GetInt(PrefKeys.AutoCooking) == 0)
            {
                return;
            }
            
            tooltipManager.SendSelectRecipe(recipeBookEntry.CurrentRecipe).Forget();
        }

        public void SwitchHighlight(List<Ingredients> ingredients)
        {
            tooltipManager.DisableAllHighlights();
            
            if (cauldron.Mix.Count != 0)
            {
                foreach (var ingredient in cauldron.Mix)
                {
                    if (ingredients.Contains(ingredient) == false)
                    {
                        TryHighlightIncorrectRecipe();
                        return;
                    }
                }
            }
            
            CloseBook();

            if (cauldron.potionPopup.IsEnable)
            {
                return;
            }

            foreach (Ingredients ingredient in ingredients)
            {
                tooltipManager.ChangeOneIngredientHighlight(ingredient, true);
            }
        }

        private void TryHighlightIncorrectRecipe()
        {
            OnSelectIncorrectRecipe?.Invoke();
            Debug.LogWarning("The ingredients in the cauldron do not match this recipe"); 
        }

        public bool IsIngredientSetKnown(Ingredients[] recipe)
        {
            //Check potions
            if (unlockedRecipes.Any(unlockedRecipe => unlockedRecipe.RecipeIngredients.All(recipe.Contains)))
            {
                return true;
            }

            //Check attempts
            if(experimentController.wrongPotions.Count != 0 && experimentController.wrongPotions.Any(wrongRecipe => wrongRecipe.IngredientsList.All(recipe.Contains)))
            {
                return true;
            }

            return false;
        }

        public void LoadData(GameData data, bool newGame)
        {
            experimentController.wrongPotions = wrongRecipeProvider.LoadWrongRecipe();
            foreach (var potion in experimentController.wrongPotions)
            {
                potion.RestoreIngredients();
            }
            experimentController.GenerateData();
        }

        public void SaveData(ref GameData data)
        {
            wrongRecipeProvider.wrongPotions = experimentController.wrongPotions;
            wrongRecipeProvider.SaveWrongRecipes();
        }

        public override void CloseBook()
        {
            DisposeEndingScreen();
            base.CloseBook();
        }

        private void DisposeEndingScreen()
        {
            if (!endingScreen)
            {
                return;
            }
            if (endingScreen.IsOpened)
            {
                endingScreen.Close();
                endingScreen.OnClose += () => Destroy(endingScreen);
            }
            else
            {
                Destroy(endingScreen);
            }
            endingScreen = null;
        }
    }
}