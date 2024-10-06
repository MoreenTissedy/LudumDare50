using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;
using static CauldronCodebase.GameStates.GameStateMachine;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/Silver")]
    public class SilverGameMode : GameModeBase
    {
        private int patience = 1;
        private List<Ingredients> freezed => gameData.ingredientsFreezed;

        private VisitorManager visitorManager;
        private TooltipManager ingredients;
        private GameStateMachine gameStates;
        private GameDataHandler gameData;

        [Inject]
        public void Construct(VisitorManager visitorManager, TooltipManager ingredients,
                                GameStateMachine gameStates, GameDataHandler gameData,
                                Cauldron cauldron)
        {
            this.visitorManager = visitorManager;
            this.ingredients = ingredients;
            this.gameStates = gameStates;
            this.gameData = gameData;
        }

        public override void Apply()
        {
            visitorManager.VisitorEnter += () => visitorManager.attemptsLeft = patience;

            gameStates.OnChangeState += TryMorningReset;
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

        private void TryMorningReset(GamePhase phase)
        {
            if (phase != GamePhase.Night) return;

            foreach (var temp in ingredients.Dict)
            {
                var ingredient = temp.Value;
                ingredient.gameObject.SetActive(true);
            }
            freezed.Clear();
        }
    }
}