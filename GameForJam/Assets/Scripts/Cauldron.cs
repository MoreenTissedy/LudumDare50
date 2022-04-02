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
            //effect
            Debug.Log("Добавлено: "+ingredient);
            Cauldron.instance.mix.Add(ingredient);
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
            Potions result = Brew();
            Debug.Log("Сварено зелье: "+result);
            GameManager.instance.EndEncounter(result);
        }
    }
}