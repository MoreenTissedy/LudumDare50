using System;
using System.Collections.Generic;

namespace CauldronCodebase
{
    [Serializable]
    public class WrongPotion
    {
        public List<string> IngredientsNameList;
        public List<Ingredients> IngredientsList;
        public WrongPotion(List<Ingredients> ingredients)
        {
            IngredientsList = ingredients;
            
            IngredientsNameList = new List<string>(3);
            foreach (var ingredient in ingredients)
            {
                IngredientsNameList.Add(ingredient.ToString());
            }
        }

        public void RestoreIngredients()
        {
            IngredientsList = new List<Ingredients>(3);
            foreach (var ingredient in IngredientsNameList)
            {
                IngredientsList.Add((Ingredients)Enum.Parse(typeof(Ingredients), ingredient));
            }
        }
    }
}