using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
public class TooltipManager : MonoBehaviour
{
    [SerializeField]IngredientsData book;
    List<IngredientDroppable> ingredientsInScene = new List<IngredientDroppable>();
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

    public void SwitchRecipeHighlight(Recipe recipe)
    {
        if (currentRecipe == recipe)
        {
            DisableRecipeHighlight(recipe);
        }
        else if (currentRecipe != recipe)
        {
            if (currentRecipe) {DisableRecipeHighlight(currentRecipe);}
            EnableRecipeHighlight(recipe);
        }
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

    void EnableRecipeHighlight(Recipe recipyToHighlight)
    {
        potionIngredients.Clear();
        potionIngredients.Add(recipyToHighlight.ingredient1);
        potionIngredients.Add(recipyToHighlight.ingredient2);
        potionIngredients.Add(recipyToHighlight.ingredient3);
        EnableFewHighlights(potionIngredients);
        currentRecipe = recipyToHighlight;
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