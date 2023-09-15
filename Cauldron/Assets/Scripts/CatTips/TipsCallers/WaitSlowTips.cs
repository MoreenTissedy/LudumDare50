using UnityEngine;
using System.Collections;
using System.Linq;
using Zenject;

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
            if (gameDataHandler.wrongPotionsCount >= WrongPotionThreshold && Random.Range(0, 100) <= ChanceToUnlock)
            {
                randomRecipe = RecipeGenerator.GenerateLockedRecipe(recipeBook).RecipeIngredients.ToArray();
            }
            else
            {
                randomRecipe = RecipeGenerator.GenerateRandomRecipe(recipeBook);
            }
            
            if (randomRecipe == null) yield break;
            
            var ingredients = randomRecipe.Select(ingredient => ingredientsData.Get(ingredient)).ToList();
        
            CatTipsValidator.ShowTips(CatTipsGenerator.CreateTipsWithIngredients(catTipsProvider.SlowPlayerTips, ingredients));

        }
    }
}