using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase.CatTips
{
    public class LastIngredientTips : TipsCaller
    {
        [Inject] private Cauldron cauldron;
        [Inject] private TooltipManager tooltipManager;
        [Inject] private IngredientsData ingredientsData;
        [Inject] private RecipeBook recipeBook;

        protected override void Start()
        {
            cauldron.IngredientAdded += TryShowCatTip;
        }

        protected override void OnDestroy()
        {
            cauldron.IngredientAdded -= TryShowCatTip;
        }
        
        private void TryShowCatTip(Ingredients ingredients)
        {
            if (cauldron.Mix.Count != 2 || tooltipManager.Highlighted) return;
            if (Random.Range(0, 3) > 0) return;
            
            Ingredients[] recipeToTips;
            Ingredients randomIngredient;

            List<Ingredients> allIngredients = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>().ToList();
            foreach (var ingredientInMix in cauldron.Mix)
            {
                allIngredients.Remove(ingredientInMix);
            }

            int tryCount = allIngredients.Count;
            do
            {
                randomIngredient = allIngredients[Random.Range(0, allIngredients.Count)];
                recipeToTips = new[] {cauldron.Mix[0], cauldron.Mix[1], randomIngredient};

                tryCount -= 1;
                if (tryCount <= 0) break;
            } while (recipeBook.IsIngredientSetKnown(recipeToTips));

            if (tryCount > 0)
            {
                CatTipsValidator.ShowTips(CatTipsGenerator.CreateTipsWithIngredient(catTipsProvider.RandomLastIngredient,
                    ingredientsData.Get(randomIngredient)));
            }
        }
    }
}