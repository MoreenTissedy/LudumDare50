using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
#pragma warning disable CS0618


namespace DefaultNamespace
{
    public class Cauldron : MonoBehaviour, IPointerClickHandler
    {
        public static Cauldron instance;

        public ParticleSystem mixColor, bubbleColor, splash;
        public float splashDelay = 2f;
        public AudioClip brew, add;
        private AudioSource audios;

        public List<Ingredients> mix;
        private void Awake()
        {
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
            Debug.Log("Добавлено: "+ingredient);
            mix.Add(ingredient);
            if (mix.Count == 3)
            {
                BrewAction();
            }
            else
            {
                RandomMixColor();
            }
        }

        public void Clear()
        {
            mix.Clear();
        }
        
        public Potions Brew()
        {
            audios.PlayOneShot(brew);
            foreach (var recipe in RecipeBook.instance.recipes)
            {
                if (mix.Contains(recipe.ingredient1) && mix.Contains(recipe.ingredient2) &&
                    mix.Contains(recipe.ingredient3))
                {
                    mix.Clear();
                    //color mix in the potion color
                    MixColor(recipe.color);
                    return recipe.potion;
                }
            }
            
            RandomMixColor();
            mix.Clear();
            return Potions.Placebo;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            BrewAction();
        }

        private void BrewAction()
        {
            Potions result = Brew();
            Witch.instance.Activate();
            Debug.Log("Сварено зелье: " + result);
            //GameManager.instance.ShowText("Сварено зелье: " + result);
            GameManager.instance.EndEncounter(result);
        }
    }
}