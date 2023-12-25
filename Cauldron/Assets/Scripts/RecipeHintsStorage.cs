using System;
using System.Collections.Generic;
using System.Linq;
using EasyLoc;
using UnityEngine;

namespace CauldronCodebase
{
    [Serializable]
    public class RecipeHintConfig
    {
        public Potions recipe = Potions.Placebo;
        [Range(1, 2)]
        public int level = 1;
    }

    [Serializable]
    public class RecipeHint
    {
        public Potions recipe;
        [TextArea(3, 3)]
        public string hint1;
        [TextArea(3, 3)]
        public string hint2;
    }
    
    [CreateAssetMenu()]
    public class RecipeHintsStorage: ScriptableObject //LocalizableSO
    {
        [SerializeField] private RecipeHint[] hints;

        public bool TryGetHint(Potions recipe, out string hint)
        {
            var list = GetHints();
            int index = FindIndexOfRecipe(recipe, list);
            if (index < 0)
            {
                hint = string.Empty;
                return false;
            }
            else
            {
                int level = int.Parse(list[index].Split(':')[1]);
                var recipeHint = hints.FirstOrDefault(x => x.recipe == recipe);
                if (recipeHint is null)
                {
                    hint = string.Empty;
                    return false;
                }
                switch (level)
                {
                    case 1:
                        hint = recipeHint.hint1;
                        return true;
                    case 2:
                        hint = recipeHint.hint2;
                        return true;
                }
                hint = string.Empty;
                return false;
            }
        }

        public void SaveHint(RecipeHintConfig config)
        {
            if (config.recipe == Potions.Placebo)
            {
                return;
            }

            var hint = hints.FirstOrDefault(x => x.recipe == config.recipe);
            if (hint is null)
            {
                return;
            }
            
            SaveHint(hint, config.level);
        }

        private static void SaveHint(RecipeHint hint, int level)
        {
            StringListWrapper tags;
            if (PlayerPrefs.HasKey(PrefKeys.RecipeHints))
            {
                var encodedTags = PlayerPrefs.GetString(PrefKeys.RecipeHints);
                tags = JsonUtility.FromJson<StringListWrapper>(encodedTags);
            }
            else
            {
                tags = new StringListWrapper();
            }

            int indexOfRecipe = FindIndexOfRecipe(hint.recipe, tags.list);
            if (indexOfRecipe >= 0)
            {
                tags.list.RemoveAt(indexOfRecipe);
            }
            tags.list.Add($"{hint.recipe}:{level}");
            string value = JsonUtility.ToJson(tags);
            Debug.Log("recipe hint saved: "+value);
            PlayerPrefs.SetString(PrefKeys.RecipeHints, value);
        }

        private static List<string> GetHints()
        {
            if (PlayerPrefs.HasKey(PrefKeys.RecipeHints))
            {
                var encodedTags = PlayerPrefs.GetString(PrefKeys.RecipeHints);
                return JsonUtility.FromJson<StringListWrapper>(encodedTags).list;
            }
            return new List<string>();
        }

        private static int FindIndexOfRecipe(Potions recipe, List<string> strings)
        {
            int foundIndex = -1;
            for (var index = 0; index < strings.Count; index++)
            {
                var content = strings[index];
                if (content.Split(':')[0] == recipe.ToString())
                {
                    foundIndex = index;
                }
            }
            return foundIndex;
        }

        /*public override bool Localize(Language language)
        {
            if (localizationCSV == null)
                return false;
            //cache??
            string[] lines = localizationCSV.text.Split('\n');
            int requiredColumn = -1;
            string[] headers = lines[0].Split(';');
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_" + language.ToString()))
                {
                    requiredColumn = i;
                    break;
                }
            }

            if (requiredColumn < 1)
            {
                return false;
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');

                foreach (var recipeHint in hints)
                {
                    if (data[0] == $"{recipeHint.recipe}.hint1")
                    {
                        recipeHint.hint1 = data[requiredColumn];
                    }
                    else if (data[0] == $"{recipeHint.recipe}.hint2")
                    {
                        recipeHint.hint2 = data[requiredColumn];
                    }
                }
            }

            return true;
        }*/
    }
}