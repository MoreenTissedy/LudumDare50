using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;
using UnityEditorInternal;


namespace CauldronCodebase
{
    public class Cauldron : MonoBehaviour
    {
        [HideInInspector] public VisitorState visitorState;

        [Inject]
        TooltipManager tooltipManager;
        
        public PotionPopup potionPopup;

        public SpriteRenderer baseMix, effectMix;
        public ParticleSystem bubbleColor, splash;
        public float splashDelay = 2f;
        public AudioClip brew, add;
        private AudioSource audios;
        private Mix mixScript;
        private Fire fireScript;
        private bool mixFound, fireFound;
        private float mixBonusTotal;
        public float mixBonusMin = 2;
        public GameObject diamond;
        public float lighterColorCoef = 0.2f;
        public float darkerColorCoef = 0.3f; 

        public List<Ingredients> mix;

        public bool IsBoiling => fireScript?.boiling ?? false;
        public bool IsMixRight => mixScript?.IsWithinKeyWindow() ?? false;


        public event Action MouseEnterCauldronZone;
        public event Action<Ingredients> IngredientAdded;
        public event Action<Potions> PotionBrewed;
        public event Action PotionDeclined;

        private RecipeProvider _recipeProvider;
        private RecipeBook recipeBook;

        private Potions currentPotionBrewed;
        private GameStateMachine gameStateMachine;

        [Inject]
        public void Construct(GameStateMachine stateMachine, RecipeProvider recipeProvider, RecipeBook book)
        {
            _recipeProvider = recipeProvider;
            recipeBook = book;
            gameStateMachine = stateMachine;            
        }
        
        private void Awake()
        {
            visitorState.NewEncounter += (i, i1) => Clear();
            fireScript = GetComponentInChildren<Fire>();
            if (fireScript is null)
                fireFound = false;
            else
            {
                fireFound = true;
            }
            mixScript = GetComponent<Mix>();
            if (mixScript is null)
                mixFound = false;
            else
            {
                mixFound = true;
            }
            splash.Stop();
            audios = GetComponent<AudioSource>();
            potionPopup.OnDecline += () => PotionDeclined?.Invoke();
        }

        private void OnValidate()
        {
            potionPopup = FindObjectOfType<PotionPopup>();
        }

        void RandomMixColor()
        {
            float randomHue = Random.value;
            Color newColor = Color.HSVToRGB(randomHue, 1, 0.7f);
            Color lighterColor = Color.HSVToRGB(randomHue, 0.8f, 0.7f);
            effectMix.color = lighterColor;
            baseMix.color = newColor;
            bubbleColor.startColor = lighterColor;
            StartCoroutine(ColorSplash(newColor));
        }

        public void MixColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            Color lighterColor = Color.HSVToRGB(h, s - lighterColorCoef, v);
            Color darkerColor = Color.HSVToRGB(h, s , v-darkerColorCoef);
            effectMix.color = darkerColor;
            baseMix.color = color;
            bubbleColor.startColor = lighterColor;
            StartCoroutine(ColorSplash(color));
        }

        IEnumerator ColorSplash(Color color)
        {
            yield return new WaitForSeconds(splashDelay);
            splash.startColor = color;
        }


        public void AddToMix(Ingredients ingredient)
        {
            //Witch.instance.Activate();
            splash.Play();
            audios.PlayOneShot(add);

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
            mix.Add(ingredient);
            IngredientAdded?.Invoke(ingredient);
            tooltipManager.DisableOneIngredient(ingredient);
            if (mix.Count == 3)
            {
                Brew();
            }
            else
            {
                mixScript.RandomJolt();
                //RandomMixColor();
            }
        }

        public void Clear()
        {
            mix.Clear();
            mixBonusTotal = 0;
            mixScript.RandomKey();
            mixScript.SetToKey();
        }

        private Potions Brew()
        {
            //Witch.instance.Activate();
            audios.PlayOneShot(brew);
            tooltipManager.DisableAllHIghlights();
            
            potionPopup.ClearAcceptSubscriptions();
            //if (mixBonusTotal > mixBonusMin)
            {
                foreach (var recipe in _recipeProvider.allRecipes)
                {
                    if (mix.Contains(recipe.ingredient1) && mix.Contains(recipe.ingredient2) &&
                        mix.Contains(recipe.ingredient3))
                    {
                        //color mix in the potion color
                        MixColor(recipe.color);
                        // Debug.Log($"Mixed {recipe.name} with bonus {mixBonusTotal-mixBonusMin}");
                        // var numDiamonds = Mathf.FloorToInt(mixBonusTotal-mixBonusMin);
                        // for (int i = 0; i < numDiamonds; i++)
                        // {
                        //     Debug.Log("Bonus!");
                        //     Instantiate(diamond, transform.position, Quaternion.identity);
                        // }
                        //if recipe is not in book -> add
                        if (!recipeBook.IsRecipeInBook(recipe))
                        {
                            potionPopup.Show(recipe, true);
                            recipeBook.RecordRecipe(recipe);
                        }
                        else
                        {
                            potionPopup.Show(recipe);
                        }
                        potionPopup.OnAccept += () => PotionAccepted(recipe.potion);
                        mix.Clear();
                        return recipe.potion;
                    }
                }
            }

            recipeBook.RecordAttempt(mix.ToArray());
            RandomMixColor();
            potionPopup.Show(null);
            potionPopup.OnAccept += () => PotionAccepted(Potions.Placebo);
            mix.Clear();
            return Potions.Placebo;
        }

        private void PotionAccepted(Potions potion)
        {
            potionPopup.ClearAcceptSubscriptions();
            PotionBrewed?.Invoke(potion);
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