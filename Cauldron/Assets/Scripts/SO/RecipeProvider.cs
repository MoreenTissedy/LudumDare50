using System;
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
        private List<int> unlockedRecipes;

        private readonly string fileName = "UnlockedRecipes";
        private FileDataHandler<ListToSave<int>> fileDataHandler;

        public void Load()
        {
            fileDataHandler  = new FileDataHandler<ListToSave<int>>(fileName);
            unlockedRecipes = LoadUnlockedRecipes();
        }

        public void SaveRecipes(IEnumerable<Recipe> set)
        {
            unlockedRecipes = set.Select(x => (int)x.potion).ToList();
            Save();
        }

        private void Save()
        {
            fileDataHandler.Save(new ListToSave<int>(unlockedRecipes));
        }

        public IEnumerable<Recipe> LoadRecipes()
        {
            if (unlockedRecipes.Count > 0)
            {
                foreach (var potion in unlockedRecipes)
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
        
        private List<int> LoadUnlockedRecipes()
        {
            if (TryLoadLegacy(out var list)) return list;
            
            return fileDataHandler.IsFileValid() ? fileDataHandler.Load().list : new List<int>();
        }

        private bool TryLoadLegacy(out List<int> list)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.UnlockedRecipes))
            {
                list = null;
                return false;
            }

            list = new List<int>();
            string data = PlayerPrefs.GetString(PrefKeys.UnlockedRecipes);
            foreach (var potion in data.Split(','))
            {
                if (string.IsNullOrWhiteSpace(potion))
                {
                    continue;
                }
                list.Add(int.Parse(potion));
            }
            PlayerPrefs.DeleteKey(PrefKeys.UnlockedRecipes);

            return true;
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