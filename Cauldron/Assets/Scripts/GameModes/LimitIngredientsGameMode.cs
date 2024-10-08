using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;
using static CauldronCodebase.GameStates.GameStateMachine;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/LimitIngredients")]
    public class LimitIngredientsGameMode : PrestigeGameMode
    {
        private int patience = 1;
        private List<Ingredients> freezed => gameData.ingredientsFreezed;

        private VisitorManager visitorManager;
        private TooltipManager ingredients;

        [Inject]
        public void Construct(VisitorManager visitorManager, TooltipManager ingredients,
                                GameDataHandler gameData, GameStateMachine gameStates,
                                IAchievementManager achievement)
        {
            Construct(gameStates, gameData, achievement);

            this.visitorManager = visitorManager;
            this.ingredients = ingredients;
        }

        public override void Apply()
        {
            base.Apply();
            achievIdents = AchievIdents.SILVER_DAYS;

            visitorManager.VisitorEntering += () => visitorManager.attemptsLeft = patience;

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