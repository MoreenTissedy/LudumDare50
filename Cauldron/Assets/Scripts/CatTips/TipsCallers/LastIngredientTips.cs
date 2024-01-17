using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase.CatTips
{
    public class LastIngredientTips : MonoBehaviour
    {
        [Inject] private Cauldron cauldron;
        [Inject] private TooltipManager tooltipManager;
        [Inject] private IngredientsData ingredientsData;
        [Inject] private RecipeBook recipeBook;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private MainSettings settings;
        [Inject] private CatTipsProvider catTipsProvider;
        [Inject] private CatTipsValidator catTipsValidator;

        private bool tooltipShown;
        protected void Start()
        {
            cauldron.IngredientAdded += TryShowCatTip;
            cauldron.PotionBrewed += HideTip;
        }

        protected void OnDestroy()
        {
            cauldron.IngredientAdded -= TryShowCatTip;
            cauldron.PotionBrewed -= HideTip;
        }

        private void HideTip(Potions obj)
        {
            if (tooltipShown)
            {
                catTipsValidator.HideTips();
                tooltipShown = false;
            }
        }

        private void TryShowCatTip(Ingredients ingredients)
        {
            if (cauldron.Mix.Count != 2 || tooltipManager.Highlighted) return;
            
            Ingredients[] randomRecipe;
            if (gameDataHandler.wrongExperiments >= settings.catTips.WrongExperimentThreshold && Random.Range(0, 100) <= settings.catTips.ChanceToUnlock)
            {
                gameDataHandler.wrongExperiments = 0;
                randomRecipe = RecipeGenerator.GenerateCorrectLastIngredientRecipe(cauldron.Mix.ToArray(), recipeBook);
            }
            else if (Random.Range(0, 100) <= settings.catTips.ChanceToFail) 
            {
                randomRecipe = RecipeGenerator.GenerateLastIngredientRecipe(cauldron.Mix.ToArray(), recipeBook);
            }
            else
            {
                return;
            }

            if (randomRecipe is null)
            {
                return;
            }

            var randomIngredient = randomRecipe.Except(cauldron.Mix).ToArray()[0];
            tooltipShown = catTipsValidator.ShowTips(CatTipsGenerator.CreateTipsWithIngredient(
                catTipsProvider.RandomLastIngredient,
                ingredientsData.Get(randomIngredient)));
            
        }
    }
}