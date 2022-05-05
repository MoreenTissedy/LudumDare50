using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

#pragma warning disable CS0618


namespace DefaultNamespace
{
    public class Cauldron : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        public static Cauldron instance;

        public ParticleSystem mixColor, bubbleColor, splash;
        public float splashDelay = 2f;
        public AudioClip brew, add;
        private AudioSource audios;
        private Mix mixScript;
        private float mixBonusTotal;
        public float mixBonusMin = 2, mixBonus1 = 3, mixBonus2 = 5;
        public GameObject diamond;

        public List<Ingredients> mix;

        public event Action mouseEnterCauldronZone;
        private void Awake()
        {
            mixScript = FindObjectOfType<Mix>();
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
            mixColor.startColor = newColor;
            bubbleColor.startColor = lighterColor;
            StartCoroutine(ColorSplash(newColor));
        }

        void MixColor(Color color)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            Color lighterColor = Color.HSVToRGB(h, s - 0.2f, v);
            mixColor.startColor = color;
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
            float bonus = Mathf.Clamp(mixScript.keyMixWindow/2/Mathf.Abs(mixScript.keyMixValue - mixScript.mixProcess), 0, 5);
            mixBonusTotal += bonus;
            Debug.Log($"Added {ingredient} with bonus {bonus}");
            mix.Add(ingredient);
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
                foreach (var recipe in RecipeBook.instance.recipes)
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
            splash.Play();
        }

        private void BrewAction()
        {
            Potions result = Brew();
            Witch.instance.Activate();
            Debug.Log("Сварено зелье: " + result);
            //GameManager.instance.ShowText("Сварено зелье: " + result);
            GameManager.instance.EndEncounter(result);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseEnterCauldronZone?.Invoke();
        }
    }
}