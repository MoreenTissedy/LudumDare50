using System;
using UnityEngine;
using EasyLoc;
using System.Collections.Generic;


namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New recipe", menuName = "Recipe", order = 0)]
    public class Recipe : LocalizableSO
    {
        public Potions potion;
        public Color color;
        public bool magical;
        public List<Ingredients> RecipeIngredients;
        public string potionName;
        [TextArea(2, 10)]
        public string description;
        public Sprite image;
        public string requiredStoryTag;

        public override bool Localize(Language language)
        {
            if (localizationCSV == null)
                return false;
            //cache??
            string[] lines = localizationCSV.text.Split('\n');
            List<int> requiredColumns = new List<int>();
            string[] headers = lines[0].Split(';');
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+language.ToString()))
                {
                    requiredColumns.Add(i);
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (data[0] == name)
                {
                    potionName = data[requiredColumns[0]];
                    description = data[requiredColumns[1]];
                    return true;
                }
            }

            return false;
        }
        
        public bool SearchRecipe(Ingredients ingredients, Ingredients ingredients1, Ingredients ingredients2)
        {
            bool isFullRecipe = false;
            
            foreach (Ingredients type in RecipeIngredients)
            {
                if (type == ingredients)
                {
                    isFullRecipe = true;
                }
                else if (type == ingredients1)
                {
                    isFullRecipe = true;
                }
                else if (type == ingredients2)
                {
                    isFullRecipe = true;
                }
                else
                {
                    return false;
                }
            }

            return isFullRecipe;
        }
    }
}