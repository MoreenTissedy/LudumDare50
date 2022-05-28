using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
public class TooltipManager : MonoBehaviour
{
    List<Ingredients> potionIngredients = new List<Ingredients>();
    Ingredients ingToH;
    Recipe currentRecipe;
    bool isHighlighted;
    Dictionary<Ingredients, IngredientDroppable> dict = new Dictionary<Ingredients, IngredientDroppable>();
    
    void Awake()
    {
        foreach(var i in FindObjectsOfType<IngredientDroppable>())
            {dict.Add(i.ingredient, i);}
    }

    public void HighlightRecipe(Recipe recipe)
    {
        if (!recipe)
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

    void DisableRecipeHighlight(Recipe recipyToDisable)
    {
        potionIngredients.Clear();
        potionIngredients.Add(recipyToDisable.ingredient1);
        potionIngredients.Add(recipyToDisable.ingredient2);
        potionIngredients.Add(recipyToDisable.ingredient3);
        DisableFewHighlights(potionIngredients);
    }
}
}