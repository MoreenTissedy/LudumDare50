using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase.GameStates;
using UnityEngine.EventSystems;
using Universal;
using Zenject;
using Random = UnityEngine.Random;


namespace CauldronCodebase
{
    public class Cauldron : MonoBehaviour
    {
        public PotionPopup potionPopup;
        public ParticleSystem splash;
        public TooltipManager tooltipManager;
        private List<Ingredients> mix = new List<Ingredients>();

        [SerializeField] public List<Ingredients> Mix => mix;
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
        private CatTipsManager catTipsManager;
        private IngredientsData ingredientsData;
        private CatTipsView catTipsView;

        [Inject]
        public void Construct(GameStateMachine gameStateMachine, RecipeProvider recipeProvider, RecipeBook recipeBook,
            SoundManager soundManager, TooltipManager tooltipManager, CatTipsManager tipsManager,
            IngredientsData ingredients, CatTipsView tipsView)
        {
            this.recipeProvider = recipeProvider;
            this.recipeBook = recipeBook;
            this.gameStateMachine = gameStateMachine;
            this.soundManager = soundManager;
            this.tooltipManager = tooltipManager;
            catTipsManager = tipsManager;
            ingredientsData = ingredients;
            catTipsView = tipsView;
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

            TryShowCatTip();
            
            if (mix.Count == 3)
            {
                Brew();
            }
        }

        
        //TODO: refactor away from here
        private void TryShowCatTip()
        {
            if (Random.Range(0, 3) > 0)
            {
                return;
            }
            if (mix.Count == 2 && !tooltipManager.Highlighted)
            {
                Ingredients[] recipeToTips;
                Ingredients randomIngredient;

                List<Ingredients> allIngredients = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>().ToList();
                foreach (var ingredientInMix in mix)
                {
                    allIngredients.Remove(ingredientInMix);
                }

                int tryCount = allIngredients.Count;
                do
                {
                    randomIngredient = allIngredients[Random.Range(0, allIngredients.Count)];
                    recipeToTips = new[] {mix[0], mix[1], randomIngredient};

                    tryCount -= 1;
                    if (tryCount <= 0) break;
                } while (recipeBook.IsIngredientSetKnown(recipeToTips));

                if (tryCount > 0)
                {
                    catTipsManager.ShowTips(CatTipsGenerator.CreateTipsWithIngredient(catTipsManager.RandomLastIngredient,
                        ingredientsData.Get(randomIngredient)));
                }
            }
        }

        public void Clear(GameStateMachine.GamePhase phase)
        {
            if (phase != GameStateMachine.GamePhase.Visitor) return;

            mix.Clear();
        }

        private Potions Brew()
        {
            catTipsView.HideView();
            soundManager.Play(Sounds.PotionReady);
            tooltipManager.DisableAllHighlights();
            potionPopup.ClearAcceptSubscriptions();
            {
                foreach (var recipe in recipeProvider.allRecipes)
                {
                    if (recipe.RecipeIngredients.All(ingredient => mix.Contains(ingredient)))
                    {
                        if (!recipeBook.IsRecipeInBook(recipe))
                        {
                            potionPopup.Show(recipe, true);
                            LoggerTool.TheOne.Log("UNLOCK: "+recipe.potionName);
                            recipeBook.RecordRecipe(recipe);
                        }
                        else
                        {
                            LoggerTool.TheOne.Log("Potion: "+recipe.potionName);
                            potionPopup.Show(recipe);
                        }

                        PotionBrewed?.Invoke(recipe.potion);
                        potionPopup.OnAccept += () => OnPotionAccepted(recipe.potion);
                        mix.Clear();
                        return recipe.potion;
                    }
                }
            }

            LoggerTool.TheOne.Log("Potion fail");
            recipeBook.RecordAttempt(new WrongPotion(mix));
            potionPopup.Show(null);
            PotionBrewed?.Invoke(Potions.Placebo);
            potionPopup.OnAccept += () => OnPotionAccepted(Potions.Placebo);
            mix.Clear();
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