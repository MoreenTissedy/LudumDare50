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

        public SpriteRenderer baseMix, effectMix;
        public ParticleSystem bubbleColor, splash;
        public float splashDelay = 2f;
        private Mix mixScript;
        private Fire fireScript;
        private bool mixFound, fireFound;
        private float mixBonusTotal;
        public float mixBonusMin = 2;

        public List<Ingredients> Mix;

        public bool IsBoiling => fireScript?.boiling ?? false;
        public bool IsMixRight => mixScript?.IsWithinKeyWindow() ?? false;


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
            fireScript = GetComponentInChildren<Fire>();
            fireFound = fireScript != null;
            mixScript = GetComponent<Mix>();
            mixFound = mixScript != null;
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
            //Witch.instance.Activate();
            splash.Play();
            soundManager.Play(Sounds.Splash);

            float bonus = 0;
            if (mixFound)
            {
                float bonusValue = mixScript.keyMixWindow / 2 / Mathf.Abs(mixScript.keyMixValue - mixScript.mixProcess);
                bonus += Mathf.Clamp(bonusValue, 0, 5);
            }

            if (fireFound && !fireScript.Boiling)
            {
                bonus = 0;
            }

            mixBonusTotal += bonus;
            Mix.Add(ingredient);
            IngredientAdded?.Invoke(ingredient);
            tooltipManager.ChangeOneIngredientHighlight(ingredient, false);
            if (Mix.Count == 3)
            {
                Brew();
            }
            else
            {
                mixScript.RandomJolt();
            }
        }

        public void Clear(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Visitor) return;

            Mix.Clear();
            mixBonusTotal = 0;
            mixScript.RandomKey();
            mixScript.SetToKey();
        }

        private Potions Brew()
        {
            //Witch.instance.Activate();
            soundManager.Play(Sounds.PotionReady);
            tooltipManager.DisableAllHighlights();

            potionPopup.ClearAcceptSubscriptions();
            //if (mixBonusTotal > mixBonusMin)
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

        public void OnPointerClick(PointerEventData eventData)
        {
            //splash.Play();
        }

        public void PointerEntered()
        {
            MouseEnterCauldronZone?.Invoke();
        }
    }
}