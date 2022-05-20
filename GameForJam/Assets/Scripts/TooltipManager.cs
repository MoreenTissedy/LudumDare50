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
    
    
    void OnValidate()
    {
        ingredientsInScene.AddRange(FindObjectsOfType<IngredientDroppable>());
        foreach(var ingredientInBook in book.book);

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
       foreach (var ing in ingredientsInScene)
       {
           if(ing.ingredient == ingredientToHighlight)
           {
               ing.EnableHighlight();
               //Debug.Log("Включил, это " + ing.ingredient);
               return; 
           }
       }
    }

    public void EnableFewHighlights(List<Ingredients> ingredientsToHighlight)
    {
        foreach(var i in ingredientsToHighlight)
        {
            foreach(var ing in ingredientsInScene)
            {
                if(ing.ingredient == i)
                {
                    ing.EnableHighlight();
                }

            }   
        }
    }

    public void DisableOneIngredient(Ingredients ingredientToHighlight)
    {
       foreach (var ing in ingredientsInScene)
       {
           if(ing.ingredient == ingredientToHighlight)
           {
               ing.DisableHighlight();
               Debug.Log("Выключил, это " + ing.ingredient);
               return; 
           }
       }
    }

    public void DisableFewHighlights(List<Ingredients> ingredientsToHighlight)
    {
        foreach(var i in ingredientsToHighlight)
        {
            foreach(var ing in ingredientsInScene)
            {
                if(ing.ingredient == i)
                {
                    ing.DisableHighlight();
                }
            }   
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