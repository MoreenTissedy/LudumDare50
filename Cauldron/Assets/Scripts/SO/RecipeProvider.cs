using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "All Recipe Set", menuName = "Recipe Deck", order = 1)]
    public class RecipeProvider : ScriptableObject
    {
        //cache to dictionary?
        public Recipe[] allRecipes;
        
        public Recipe GetRecipeForPotion(Potions potion)
        {
            var found = allRecipes.Where(x => x.potion == potion).ToArray();
            if (found.Length > 0)
            {
                return found[0];
            }
            else
            {
                return null;
            }
        }
        
        
        [ContextMenu("Export Recipes to CSV")]
        public void ExportRecipes()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Recipes.csv");
            file.WriteLine("id;name_RU;description_RU;name_EN;description_EN");
            foreach (var recipe in allRecipes)
            {
                file.WriteLine(recipe.name+";"+recipe.potionName+";"+recipe.description);
            }
            file.Close();
        }
    }
}