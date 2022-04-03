using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace DefaultNamespace
{
    public class Cauldron : MonoBehaviour, IPointerClickHandler
    {
        public static Cauldron instance;

        private void Awake()
        {
            if (instance is null)
                instance = this;
            else
            {
                Debug.LogError("double singleton:"+this.GetType().Name);
            }
            
        }
        
        public List<Ingredients> mix;

        public void AddToMix(Ingredients ingredient)
        {
            Witch.instance.Activate();
            //effect
            Debug.Log("Добавлено: "+ingredient);
            mix.Add(ingredient);
            if (mix.Count == 3)
            {
                BrewAction();
            }
        }

        public void Clear()
        {
            mix.Clear();
        }
        
        public Potions Brew()
        {
            //effects
            
            foreach (var recipe in RecipeBook.instance.recipes)
            {
                if (mix.Contains(recipe.ingredient1) && mix.Contains(recipe.ingredient2) &&
                    mix.Contains(recipe.ingredient3))
                {
                    mix.Clear();
                    return recipe.potion;
                }
            }

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