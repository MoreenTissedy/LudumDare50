using System.Collections.Generic;
using Zenject;

namespace CauldronCodebase
{
public class TooltipManager
{
    public bool Highlighted { get; private set; }
    private List<Ingredients> potionIngredients = new List<Ingredients>();
    private Recipe currentRecipe;
    private Dictionary<Ingredients, IngredientDroppable> dict;

    [Inject] private Cauldron cauldron;

    public TooltipManager()
    {
        dict = new Dictionary<Ingredients, IngredientDroppable>();
    }

    public void AddIngredient(IngredientDroppable button)
    {
        dict.Add(button.ingredient, button);
    }

    public void RemoveIngredient(IngredientDroppable button)
    {
        if (!dict.ContainsKey(button.ingredient)) return;

        dict.Remove(button.ingredient);
    }

    public void HighlightRecipe(Recipe recipe)
    {
        if (recipe is null) return;
        
        if (currentRecipe != null)
        {
            ChangeFewHighlights(currentRecipe,false);
        }
        ChangeFewHighlights(recipe, true);
        currentRecipe = recipe;
        Highlighted = true;
    }
    
    public void ChangeOneIngredientHighlight(Ingredients ingredientToHighlight, bool state)
    {
        if (dict.TryGetValue(ingredientToHighlight, out var ingredient))
        {
            ingredient.ChangeHighlight(state);
        }
    }

    private void ChangeFewHighlights(Recipe recipe, bool state)
    {
        potionIngredients = recipe.RecipeIngredients;

        foreach(var i in potionIngredients)
        {
            if(cauldron.Mix.Contains(i)) continue;

            if (dict.TryGetValue(i, out var temp))
            {
                temp.ChangeHighlight(state);
            }
        }
    }
    
    public void DisableAllHighlights()
    {
        foreach(var i in dict.Keys)
        {
            if (dict.TryGetValue(i, out var ingredient))
            {
                ingredient.ChangeHighlight(false);
                Highlighted = false;
            }
        }
    }
}
}