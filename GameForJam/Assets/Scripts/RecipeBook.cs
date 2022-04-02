using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class RecipeBook : MonoBehaviour
    {
        public static RecipeBook instance;

        private void Awake()
        {
            if (instance is null)
                instance = this;
            else
            {
                Debug.LogError("double singleton:"+this.GetType().Name);
            }
            
        }
        
        public Recipe[] recipes;

        public Recipe GetRecipeForPotion(Potions potion)
        {
            return recipes.Where(x => x.potion == potion).ToArray()[0];
        }
    }
}