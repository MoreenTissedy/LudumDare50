using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase.GameStates;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;


namespace CauldronCodebase
{
   
    public class Cauldron : MonoBehaviour
    {
        [Inject] public TooltipManager tooltipManager;

        public PotionPopup potionPopup;
        public ParticleSystem splash;
        public List<Ingredients> Mix;
        
        public event Action MouseEnterCauldronZone;
        public event Action<Ingredients> IngredientAdded;
        public event Action<Potions> PotionBrewed;
        public event Action<Potions> PotionAccepted;
        public event Action PotionDeclined;

        private RecipeProvider recipeProvider;
        private RecipeBook recipeBook;

        private Potions currentPotionBrewed;
        private GameStateMachine gameStateMachine;
        private SoundManager soundManager;

        [Inject]
        public void Construct(GameStateMachine stateMachine, RecipeProvider recipeProvider, RecipeBook book,
            SoundManager sounds)
        {
            this.recipeProvider = recipeProvider;
            recipeBook = book;
            gameStateMachine = stateMachine;
            soundManager = sounds;
        }

        private void Awake()
        {
            gameStateMachine.OnChangeState += Clear;
            splash.Stop();
            potionPopup.OnDecline += () => PotionDeclined?.Invoke();
        }

        private void OnDestroy()
        {
            gameStateMachine.OnChangeState -= Clear;
        }

        private void OnValidate()
        {
            potionPopup = FindObjectOfType<PotionPopup>();
        }


        public void AddToMix(Ingredients ingredient)
        {
            splash.Play();
            soundManager.Play(Sounds.Splash);
            Mix.Add(ingredient);
            IngredientAdded?.Invoke(ingredient);
            tooltipManager.ChangeOneIngredientHighlight(ingredient, false);
            if (Mix.Count == 3)
            {
                Brew();
            }
        }

        public void Clear(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Visitor) return;

            Mix.Clear();
        }

        private Potions Brew()
        {
            soundManager.Play(Sounds.PotionReady);
            tooltipManager.DisableAllHighlights();
            potionPopup.ClearAcceptSubscriptions();
            {
                foreach (var recipe in recipeProvider.allRecipes)
                {
                    if (recipe.RecipeIngredients.All(ingredient => Mix.Contains(ingredient)))
                    {
                        if (!recipeBook.IsRecipeInBook(recipe))
                        {
                            potionPopup.Show(recipe, true);
                            recipeBook.RecordRecipe(recipe);
                        }
                        else
                        {
                            potionPopup.Show(recipe);
                        }

                        PotionBrewed?.Invoke(recipe.potion);
                        potionPopup.OnAccept += () => OnPotionAccepted(recipe.potion);
                        Mix.Clear();
                        return recipe.potion;
                    }
                }
            }

            recipeBook.RecordAttempt(Mix.ToArray());
            potionPopup.Show(null);
            PotionBrewed?.Invoke(Potions.Placebo);
            potionPopup.OnAccept += () => OnPotionAccepted(Potions.Placebo);
            Mix.Clear();
            return Potions.Placebo;
        }
        
        private void OnPotionAccepted(Potions potion)
        {
            potionPopup.ClearAcceptSubscriptions();
            PotionAccepted?.Invoke(potion);
        }
        
        public void PointerEntered()
        {
            MouseEnterCauldronZone?.Invoke();
        }
    }
}