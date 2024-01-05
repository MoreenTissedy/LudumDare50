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
            IngredientsList = new List<Ingredients>(ingredients);
            
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
        
        public bool SearchIngredient(Ingredients ingredients)
        {
            foreach (Ingredients type in IngredientsList)
            {
                if (type == ingredients)
                {
                    return true;
                }
            }

            return false;
        }
    }
}