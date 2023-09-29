using UnityEngine;
using System.Collections;
using System.Linq;
using Zenject;

namespace CauldronCodebase.CatTips
{
    public class WaitSlowTips : EncounterTipsCaller
    {
        [Inject] private TooltipManager tooltipManager;
        [Inject] private RecipeBook recipeBook;
        [Inject] private IngredientsData ingredientsData;
        [Inject] private Cauldron cauldron;
        protected override bool TipShown { get; set; }

        protected override IEnumerator CallTips()
        {
            yield return new WaitForSeconds(settings.catTips.SlowTipsDelay);

            if (tooltipManager.Highlighted || recipeBook.IsOpen
                                           || cauldron.Mix.Count != 0 || cauldron.potionPopup.gameObject.activeSelf)
            {
                yield break;
            }

            Ingredients[] randomRecipe;
            if (gameDataHandler.wrongExperiments >= settings.catTips.WrongExperimentThreshold &&
                Random.Range(0, 100) <= settings.catTips.ChanceToUnlock)
            {
                gameDataHandler.wrongExperiments = 0;
                randomRecipe = RecipeGenerator.GenerateLockedRecipe(recipeBook).RecipeIngredients.ToArray();
            }
            else if (Random.Range(0, 100) <= settings.catTips.ChanceToFail)
            {
                randomRecipe = RecipeGenerator.GenerateRandomRecipe(recipeBook);
            }
            else
            {
                yield break;
            }

            var ingredients = randomRecipe.Select(ingredient => ingredientsData.Get(ingredient)).ToList();

            TipShown = catTipsValidator.ShowTips(
                CatTipsGenerator.CreateTipsWithIngredients(catTipsProvider.SlowPlayerTips, ingredients));
        }
    }
}