using System;
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
        private List<Ingredients> freez => gameData.ingredientsFreez;

        private VisitorManager visitorManager;        
        private TooltipManager ingredients;
        private GameStateMachine gameStates;
        private GameDataHandler gameData;

        [Inject]
        public void Construct(VisitorManager visitorManager, TooltipManager ingredients, GameStateMachine gameStates, GameDataHandler gameData)
        {
            this.visitorManager = visitorManager;
            this.ingredients = ingredients;
            this.gameStates = gameStates;
            this.gameData = gameData;
        }

        public override void Apply()
        {
            visitorManager.VisitorEnter += (Villager villager) => villager.patience = patience;
            gameStates.OnChangeState += TryMorningReset;

            foreach (var temp in ingredients.Dict)
            {
                var ingredient = temp.Value;
                ingredient.IngredientAdded += FreezIngredient;

                if (freez.Contains(ingredient.ingredient))
                {
                    ingredient.gameObject.SetActive(false);
                }
            }
        }

        private void FreezIngredient(IngredientDroppable ingredient)
        {
            ingredient.gameObject.SetActive(false);
            freez.Add(ingredient.ingredient);
        }

        private void TryMorningReset(GamePhase phase)
        {
            if (phase != GamePhase.Night) return;

            foreach (var temp in ingredients.Dict)
            {
                var ingredientDrop = temp.Value;
                ingredientDrop.gameObject.SetActive(true);
            }
            freez.Clear();
        }
    }
}