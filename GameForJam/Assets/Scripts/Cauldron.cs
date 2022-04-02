using System.Linq;
using UnityEngine;
using System.Collections.Generic;


namespace DefaultNamespace
{
    public class Cauldron : MonoBehaviour
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
            Cauldron.instance.mix.Add(ingredient);
        }
        
        public Potions Brew()
        {
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
    }
}