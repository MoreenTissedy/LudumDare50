using System.Linq;
using UnityEngine;

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
        
        public Ingredients[] mix;

        
        public Potions Brew()
        {
            foreach (var recipe in RecipeBook.instance.recipes)
            {
                if (mix.Contains(recipe.ingredient1) && mix.Contains(recipe.ingredient2) &&
                    mix.Contains(recipe.ingredient3))
                {
                    return recipe.potion;
                }
            }

            return Potions.Placebo;
        }
    }
}