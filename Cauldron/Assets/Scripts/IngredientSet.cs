using System;
using System.Linq;
using CauldronCodebase;

public class IngredientSet: IEquatable<IngredientSet>, IComparable<IngredientSet>
{
    public IngredientSetType SetType;
    public Recipe Recipe;
    public Ingredients[] Ingredients = new Ingredients[3];

    public bool Equals(IngredientSet other)
    {
        if (other is null)
        {
            return false;
        }
        foreach (Ingredients type in Ingredients)
        {
            if (!other.Ingredients.Contains(type))
            {
                return false;
            }
        }
        return true;
    }

    public int CompareTo(IngredientSet other)
    {
        int typeComparison = SetType.CompareTo(other.SetType);
        if (typeComparison != 0)
        {
            return typeComparison;
        }
        for (int i = 0; i < 3; i++)
        {
            var ingredientComparison = Ingredients[i].CompareTo(other.Ingredients[i]);
            if (ingredientComparison != 0)
            {
                return ingredientComparison;
            }
        }
        return 0;
    }
}