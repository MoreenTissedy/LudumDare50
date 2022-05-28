using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
public class TooltipManager : MonoBehaviour
{
    private List<Ingredients> potionIngredients = new List<Ingredients>();
    private Recipe currentRecipe;
    private Dictionary<Ingredients, IngredientDroppable> dict;
    
    void Awake()
    {
        dict = new Dictionary<Ingredients, IngredientDroppable>();
        foreach (var i in FindObjectsOfType<IngredientDroppable>())
        {
            dict.Add(i.ingredient, i);
        }
    }

    public void HighlightRecipe(Recipe recipe)
    {
        if (recipe is null)
        {
            return;
        }
        if (currentRecipe)
        {
            DisableRecipeHighlight(currentRecipe);
        }
        EnableRecipeHighlight(recipe);
    }
    
    public void EnableOneHighlight(Ingredients ingredientToHighlight)
    {
        if (dict.TryGetValue(ingredientToHighlight, out var temp))
            {temp.EnableHighlight();}
    }

    public void EnableFewHighlights(List<Ingredients> ingredientsToHighlight)
    {
        foreach(var i in ingredientsToHighlight)
        {
            if(dict.TryGetValue(i, out var temp))
            {temp.EnableHighlight();}
        }
    }

    public void DisableOneIngredient(Ingredients ingredientToHighlight)
    {
        if(dict.TryGetValue(ingredientToHighlight, out var temp))
            {temp.DisableHighlight();}
    }

    public void DisableFewHighlights(List<Ingredients> ingredientsToHighlight)
    {
        foreach(var i in ingredientsToHighlight)
        {
            if(dict.TryGetValue(i, out var temp))
                {temp.DisableHighlight();}
        }
    }

    public void DisableAllHIghlights()
    {
        foreach(var i in dict.Keys)
        {
            if(dict.TryGetValue(i, out var temp))
                {temp.DisableHighlight();}
        }
    }

    void EnableRecipeHighlight(Recipe recipeToHighlight)
    {
        potionIngredients.Clear();
        potionIngredients.Add(recipeToHighlight.ingredient1);
        potionIngredients.Add(recipeToHighlight.ingredient2);
        potionIngredients.Add(recipeToHighlight.ingredient3);
        EnableFewHighlights(potionIngredients);
        currentRecipe = recipeToHighlight;
    }

    void DisableRecipeHighlight(Recipe recipeToDisable)
    {
        potionIngredients.Clear();
        potionIngredients.Add(recipeToDisable.ingredient1);
        potionIngredients.Add(recipeToDisable.ingredient2);
        potionIngredients.Add(recipeToDisable.ingredient3);
        DisableFewHighlights(potionIngredients);
    }
}
}