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
        [SerializeField] protected RecipeBookEntry[] entries;
        public List<Recipe> recipes;
        [SerializeField] protected Text prevPageNum, nextPageNum;
        public event Action<Recipe> OnSelectRecipe;

        [Inject]
        private TooltipManager tooltipManager;

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Space))
                ToggleBook();
        }

        protected override void InitTotalPages()
        {
            totalPages = Mathf.CeilToInt((float)recipes.Count / entries.Length);
        }

        protected override void UpdatePage()
        {
            for (int i = 0; i < entries.Length; i++)
            {
                int num = currentPage*entries.Length + i;
                if (num < recipes.Count)
                {
                    entries[i].Display(recipes[num]);
                }
                else
                {
                    entries[i].Clear();
                }
            }
            nextPageNum.text = (currentPage *2+2).ToString();
            prevPageNum.text = (currentPage*2+1).ToString();
        }

        public void SwitchHighlight(RecipeBookEntry recipeBookEntry)
        {
            CloseBook();
            tooltipManager.HighlightRecipe(recipeBookEntry.CurrentRecipe);
            OnSelectRecipe?.Invoke(recipeBookEntry.CurrentRecipe);
        }
    }
}