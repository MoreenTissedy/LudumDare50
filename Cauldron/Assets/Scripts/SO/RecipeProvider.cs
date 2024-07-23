using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "All Recipe Set", menuName = "Recipe Deck", order = 1)]
    public class RecipeProvider : ScriptableObject
    {
        [ReorderableList]
        //cache to dictionary?
        public Recipe[] allRecipes;
        private PlayerProgressProvider progressProvider;
        private List<int> unlocked;

        public void Init(PlayerProgressProvider progressProvider)
        {
            this.progressProvider = progressProvider;
            unlocked = progressProvider.GetUnlockedRecipes();
        }

        public void SaveRecipes(IEnumerable<Recipe> set)
        {
            unlocked = set.Select(x => (int)x.potion).ToList();
            progressProvider.SaveRecipes(unlocked);
        }

        public IEnumerable<Recipe> LoadRecipes()
        {
            if (unlocked.Count > 0)
            {
                foreach (var potion in unlocked)
                {
                    Recipe recipe = GetRecipeForPotion((Potions) potion);
                    if (recipe != null)
                    {
                        yield return recipe;
                        //Debug.Log("recipe loaded: "+recipe.potionName);
                    }
                    else
                    {
                        Debug.LogWarning("can't load recipe by number "+potion);
                    }
                }
            }
        }

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
        
        public Recipe GetRecipeToUnlock(RecipeBook book, GameDataHandler gameDataHandler)
        {
            foreach (var recipe in allRecipes)
            {
                if (recipe.magical && !book.IsRecipeInBook(recipe) &&
                    StoryTagHelper.Check(recipe.requiredStoryTag, gameDataHandler))
                {
                    return recipe;
                }
            }
            return null;
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