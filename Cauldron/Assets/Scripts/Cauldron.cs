using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;
#pragma warning disable CS0618


namespace CauldronCodebase
{
    public class Cauldron : MonoBehaviour
    {
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

        private RecipeProvider recipeProvider;
        private RecipeBook recipeBook;
        private GameManager gm;

        [Inject]
        public void Construct(GameManager gm, RecipeProvider recipeProvider, RecipeBook book)
        {
            this.recipeProvider = recipeProvider;
            recipeBook = book;
            this.gm = gm;
            gm.NewEncounter += (i, i1) => Clear();
        }
        
        private void Awake()
        {
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
            Debug.Log($"Added {ingredient} with bonus {bonus}");
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
            
            //if (mixBonusTotal > mixBonusMin)
            {
                foreach (var recipe in recipeProvider.allRecipes)
                {
                    if (mix.Contains(recipe.ingredient1) && mix.Contains(recipe.ingredient2) &&
                        mix.Contains(recipe.ingredient3))
                    {
                        mix.Clear();
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
                        if (!recipeBook.magicalRecipes.Contains(recipe))
                        {
                            potionPopup.Show(recipe, true);
                            recipeBook.RecordRecipe(recipe);
                        }
                        else
                        {
                            potionPopup.Show(recipe);
                        }
                        PotionBrewed?.Invoke(recipe.potion);
                        return recipe.potion;
                    }
                }
            }

            recipeBook.RecordAttempt(mix.ToArray());
            RandomMixColor();
            mix.Clear();
            potionPopup.Show(null);
            PotionBrewed?.Invoke(Potions.Placebo);
            return Potions.Placebo;
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