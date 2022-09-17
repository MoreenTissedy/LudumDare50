using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBook : Book
    {   
        [Header("Recipe Book")]
        [SerializeField] protected RecipeBookEntry[] recipeEntries;
        [SerializeField] protected AttemptEntry[] attemptEntries;
        public List<Recipe> magicalRecipes;
        public List<Recipe> herbalRecipes;
        public List<Ingredients[]> attempts;
        [SerializeField] protected Text prevPageNum, nextPageNum;
        [SerializeField] private GameObject recipesDisplay, attemptsDisplay;
        public event Action<Recipe> OnSelectRecipe;

        [Inject]
        private TooltipManager tooltipManager;

        private Mode currentMode = Mode.Magical;
        public enum Mode
        {
            Magical,
            Herbal,
            Attempts
        }

        [ContextMenu("Find Entries")]
        void FindEntries()
        {
            attemptEntries = attemptsDisplay.GetComponentsInChildren<AttemptEntry>();
            recipeEntries = recipesDisplay.GetComponentsInChildren<RecipeBookEntry>();
        }

        private void Start()
        {
            ChangeMode(Mode.Magical);
        }

        public void RecordAttempt(Ingredients[] mix)
        {
            if (attempts is null)
            {
                attempts = new List<Ingredients[]>(10);
            }
            attempts.Add(mix);
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
        }

        public void ChangeMode(Mode newMode)
        {
            if (currentMode != newMode)
            {
                if (newMode == Mode.Attempts)
                {
                    recipesDisplay.SetActive(false);
                    attemptsDisplay.SetActive(true);
                }
                else if (currentMode == Mode.Attempts)
                {
                    recipesDisplay.SetActive(true);
                    attemptsDisplay.SetActive(false);
                }
                currentMode = newMode;
                currentPage = 0;
                InitTotalPages();
                UpdatePage();
            }
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
                        totalPages = Mathf.CeilToInt((float) herbalRecipes.Count / recipeEntries.Length);
                    else totalPages = 1;
                    break;
                case Mode.Attempts:
                    if (attempts != null) totalPages = Mathf.CeilToInt((float) attempts.Count / attemptEntries.Length);
                    else totalPages = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void UpdatePage()
        {
            void DisplaySet(List<Recipe> set)
            {
                if (set is null || set.Count == 0)
                {
                    foreach (var entry in recipeEntries)
                    {
                        entry.Clear();
                    }
                    return;   
                }
                for (int i = 0; i < recipeEntries.Length; i++)
                {
                    int num = currentPage * recipeEntries.Length + i;
                    if (num < set.Count)
                    {
                        recipeEntries[i].Display(set[num]);
                    }
                    else
                    {
                        recipeEntries[i].Clear();
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
            nextPageNum.text = (currentPage *2+2).ToString();
            prevPageNum.text = (currentPage*2+1).ToString();
        }

        private void DisplayAttempts()
        {
            if (attempts is null || attempts.Count == 0)
                return;
            for (int i = 0; i < attemptEntries.Length; i++)
            {
                int num = currentPage * recipeEntries.Length + i;
                if (num < attempts.Count)
                {
                    attemptEntries[i].Display(attempts[num]);
                }
                else
                {
                    attemptEntries[i].Clear();
                }
            }
        }

        public void SwitchHighlight(RecipeBookEntry recipeBookEntry)
        {
            CloseBook();
            tooltipManager.HighlightRecipe(recipeBookEntry.CurrentRecipe);
            OnSelectRecipe?.Invoke(recipeBookEntry.CurrentRecipe);
        }
    }
}