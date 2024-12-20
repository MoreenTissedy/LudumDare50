using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase.GameStates;
using Zenject;
using Random = UnityEngine.Random;


namespace CauldronCodebase
{
    public class Cauldron : MonoBehaviour
    {
        public PotionPopup potionPopup;
        public ParticleSystem splash;
        public TooltipManager tooltipManager;
        public GameObject dropZone;
        public bool IsActive => dropZone.activeInHierarchy;
        public GameObject visual;
        public ParticleSystem skinChangeVFX;
        private List<Ingredients> mix = new List<Ingredients>();

        public List<Ingredients> Mix => mix;
        public event Action MouseEnterCauldronZone;
        public event Action<Ingredients> IngredientAdded;
        public event Action<Potions> PotionBrewed;
        public event Action<Potions> PotionAccepted;
        public event Action PotionDeclined;

        private RecipeProvider recipeProvider;
        private RecipeBook recipeBook;

        private GameStateMachine gameStateMachine;
        private SoundManager soundManager;
        private GameDataHandler game;
        private ExperimentController experimentController;

        [Inject]
        public void Construct(GameStateMachine gameStateMachine, RecipeProvider recipeProvider, RecipeBook recipeBook,
            SoundManager soundManager, TooltipManager tooltipManager, GameDataHandler game, ExperimentController experimentController)
        {
            this.recipeProvider = recipeProvider;
            this.recipeBook = recipeBook;
            this.gameStateMachine = gameStateMachine;
            this.soundManager = soundManager;
            this.tooltipManager = tooltipManager;
            this.game = game;
            this.experimentController = experimentController;
        }

        private void Awake()
        {
            gameStateMachine.OnChangeState += ClearAndActivate;
            splash.Stop();
            potionPopup.OnDecline += OnPotionDecline;
        }

        private void OnDestroy()
        {
            gameStateMachine.OnChangeState -= ClearAndActivate;
        }

        private void OnValidate()
        {
            potionPopup = FindObjectOfType<PotionPopup>();
        }

        private void OnPotionDecline()
        {
            dropZone.SetActive(true);
            PotionDeclined?.Invoke();
        }

        public void AddToMix(Ingredients ingredient)
        {
            Mix.Add(ingredient);
            IngredientAdded?.Invoke(ingredient);
            tooltipManager.ChangeOneIngredientHighlight(ingredient, false);

            if (mix.Count == 3)
            {
                Brew();
            }
            else
            {
                splash.Play();
                soundManager.Play(Sounds.Splash);
            }
        }
        
        public void ClearAndActivate(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Visitor) return;

            mix.Clear();
            dropZone.SetActive(true);
        }

        private Potions Brew()
        {
            //soundManager.Play(Sounds.PotionReady);
            dropZone.SetActive(false);
            tooltipManager.DisableAllHighlights();
            potionPopup.ClearAcceptSubscriptions();
            {
                foreach (var recipe in recipeProvider.allRecipes)
                {
                    if (recipe.RecipeIngredients.All(ingredient => mix.Contains(ingredient)))
                    {
                        if (!StoryTagHelper.Check(recipe.requiredStoryTag, game))
                        {
                            continue;
                        }
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
                        mix.Clear();
                        recipeBook.CheckExperimentsCompletion();
                        return recipe.potion;
                    }
                }
            }

            experimentController.RecordAttempt(new WrongPotion(mix));
            game.wrongExperiments++;
            potionPopup.Show(null);
            PotionBrewed?.Invoke(Potions.Placebo);
            potionPopup.OnAccept += () => OnPotionAccepted(Potions.Placebo);
            mix.Clear();
            recipeBook.CheckExperimentsCompletion();
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

        public void ChangeVisual(GameObject visualPrefab)
        {
            skinChangeVFX.Play();
            if (visual != null) Destroy(visual);
            visual = Instantiate(visualPrefab, gameObject.transform);
        }
    }
}