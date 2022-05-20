using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Audio;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

#pragma warning disable CS0618


namespace DefaultNamespace
{
    public class Cauldron : MonoBehaviour
    {
        [SerializeField] TooltipManager tooltipManager;
        public static Cauldron instance;

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

        public event Action mouseEnterCauldronZone;
        public event Action ingredientAdded;
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
            // if (instance is null)
            //     instance = this;
            // else
            // {
            //     Debug.LogError("double singleton:"+this.GetType().Name);
            // }
            instance = this;
            splash.Stop();
            audios = GetComponent<AudioSource>();
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
            ingredientAdded?.Invoke();
            tooltipManager.DisableOneIngredient(ingredient);
            if (mix.Count == 3)
            {
                BrewAction();
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
        
        public Potions Brew()
        {
            audios.PlayOneShot(brew);
            
            if (mixBonusTotal > mixBonusMin)
            {
                foreach (var recipe in RecipeSet.instance.allRecipes)
                {
                    if (mix.Contains(recipe.ingredient1) && mix.Contains(recipe.ingredient2) &&
                        mix.Contains(recipe.ingredient3))
                    {
                        mix.Clear();
                        //color mix in the potion color
                        //MixColor(recipe.color);
                        Debug.Log($"Mixed {recipe.name} with bonus {mixBonusTotal-mixBonusMin}");
                        var numDiamonds = Mathf.FloorToInt(mixBonusTotal-mixBonusMin);
                        for (int i = 0; i < numDiamonds; i++)
                        {
                            Debug.Log("Bonus!");
                            Instantiate(diamond, transform.position, Quaternion.identity);
                        }
                        //if recipe is not in book -> add
                        if (!RecipeBook.instance.recipes.Contains(recipe))
                        {
                            RecipeBook.instance.recipes.Add(recipe);
                        }
                        return recipe.potion;
                    }
                }
            }

            //RandomMixColor();
            mix.Clear();
            return Potions.Placebo;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //splash.Play();
        }

        private void BrewAction()
        {
            Potions result = Brew();
            Witch.instance.Activate();
            Debug.Log("Сварено зелье: " + result);
            //GameManager.instance.ShowText("Сварено зелье: " + result);
            GameManager.instance.EndEncounter(result);
        }


        public void PointerEntered()
        {
            mouseEnterCauldronZone?.Invoke();
        }
    }
}