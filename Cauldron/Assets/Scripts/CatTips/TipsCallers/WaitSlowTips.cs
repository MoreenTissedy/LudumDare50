using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using Random = System.Random;

namespace CauldronCodebase.CatTips
{
    public class WaitSlowTips : TipsCaller
    {
        [Inject] private TooltipManager tooltipManager;
        [Inject] private RecipeBook recipeBook;
        [Inject] private IngredientsData ingredientsData;
        [Inject] private Cauldron cauldron;

        protected override void CallTips()
        {
            StartCoroutine(WaitSlow());
        }

        private IEnumerator WaitSlow()
        {
            yield return new WaitForSeconds(settings.catTips.SlowTipsDelay);
        
            if (tooltipManager.Highlighted || recipeBook.IsOpen 
                || cauldron.Mix.Count != 0 || cauldron.potionPopup.gameObject.activeSelf)
            {
                yield break;
            }

            Ingredients[] randomRecipe;
            List<Ingredients[]> generatedRecipe = new List<Ingredients[]>(RecipeBook.MAX_COMBINATIONS_COUNT);
            int tryCount = 0;

            do
            {
                randomRecipe = GenerateRandomRecipe();

                if (generatedRecipe.Any(recipe => recipe.All(randomRecipe.Contains)))
                {
                    continue;
                }

                tryCount++;
                generatedRecipe.Add(randomRecipe);

                if (tryCount >= RecipeBook.MAX_COMBINATIONS_COUNT)
                {
                    yield break;
                }
            
            } while (recipeBook.IsIngredientSetKnown(randomRecipe));
            
            var ingredients = randomRecipe.Select(ingredient => ingredientsData.Get(ingredient)).ToList();
        
            CatTipsValidator.ShowTips(CatTipsGenerator.CreateTipsWithIngredients(catTipsProvider.SlowPlayerTips, ingredients));
        }

        private Ingredients[] GenerateRandomRecipe()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
        
            var ingredients = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>().ToList();

            return ingredients.OrderBy(x => rnd.Next()).Take(3).ToArray();

        }
    }
}