using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase.GameStates;
using Zenject;

namespace CauldronCodebase.CatTips
{
    public class WaitSlowTips : TipsCaller
    {
        [Inject] private TooltipManager tooltipManager;
        [Inject] private RecipeBook recipeBook;
        [Inject] private IngredientsData ingredientsData;

        protected override void CallTips(GameStateMachine.GamePhase gamePhase)
        {
            base.CallTips(gamePhase);

            StartCoroutine(WaitSlow());
        }

        private IEnumerator WaitSlow()
        {
            yield return new WaitForSeconds(settings.catTips.SlowTipsDelay);
        
            if (tooltipManager.Highlighted || recipeBook.IsOpen)
            {
                yield break;
            }

            Ingredients[] randomRecipe;
            List<Ingredients[]> generatedRecipe = new List<Ingredients[]>(RecipeBook.MAX_COMBINATIONS_COUNT);
            int tryCount = 0;

            do
            {
                randomRecipe = recipeBook.GenerateRandomRecipe();

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
    }
}