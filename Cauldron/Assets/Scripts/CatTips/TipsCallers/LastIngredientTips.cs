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

            
            Ingredients[] randomRecipe;
            if (gameDataHandler.wrongPotionsCount >= WrongPotionThreshold && Random.Range(0, 100) <= ChanceToUnlock)
            {
                randomRecipe = RecipeGenerator.GenerateCorrectLastIngredientRecipe(cauldron.Mix.ToArray(), recipeBook);
            }
            else
            {
                randomRecipe = RecipeGenerator.GenerateLastIngredientRecipe(cauldron.Mix.ToArray(), recipeBook);
            }

            if (randomRecipe == null) return;

            var randomIngredient = randomRecipe.Except(cauldron.Mix).ToArray()[0];
            CatTipsValidator.ShowTips(CatTipsGenerator.CreateTipsWithIngredient(catTipsProvider.RandomLastIngredient,
                    ingredientsData.Get(randomIngredient)));
            
        }
    }
}