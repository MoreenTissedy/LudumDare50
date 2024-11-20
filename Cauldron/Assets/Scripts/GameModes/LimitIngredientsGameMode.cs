using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/LimitIngredients")]
    public class LimitIngredientsGameMode : PrestigeGameMode
    {
        private int patience = 1;
        private List<Ingredients> freezed => gameData.ingredientsFreezed;

        protected override string achievIdents => AchievIdents.SILVER_DAYS;

        private VisitorManager visitorManager;
        private TooltipManager ingredients;

        [Inject]
        public void Construct(VisitorManager visitorManager, TooltipManager ingredients)
        {
            this.visitorManager = visitorManager;
            this.ingredients = ingredients;
        }

        protected override void OnApply()
        {
            visitorManager.overridePatience = patience;

            gameStates.OnNewDay += TryMorningReset;
            LoadFreezedIngredients();            
        }

        private void LoadFreezedIngredients()
        {
            foreach (var temp in ingredients.Dict)
            {
                var ingredient = temp.Value;
                ingredient.IngredientAdded += FreezIngredient;

                if (freezed.Contains(ingredient.ingredient))
                {
                    ingredient.gameObject.SetActive(false);
                }
            }
        }

        private void FreezIngredient(IngredientDroppable ingredient)
        {
            ingredient.gameObject.SetActive(false);
            freezed.Add(ingredient.ingredient);
        }

        private void TryMorningReset()
        {
            foreach (var temp in ingredients.Dict)
            {
                var ingredient = temp.Value;
                ingredient.gameObject.SetActive(true);
            }
            freezed.Clear();
        }
    }
}