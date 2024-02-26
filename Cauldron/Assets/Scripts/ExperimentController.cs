using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using EasyLoc;
using NaughtyAttributes;
using UnityEngine;

public enum IngredientSetType
{
    Unknown,
    Recipe,
    Failure 
}

public class ExperimentController : MonoBehaviour
{
    [Localize] public string leafs;
    [Localize] public string roots;
    [Localize] public string mushrooms;
    [Localize] public string animals;
    
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private IngredientTypeFilter[] filters;

    public List<IngredientSet> currentRecipes;
    [ReorderableList]
    public List<AttemptEntry> attemptEntries;
    public List<WrongPotion> wrongPotions;

    private const int MaxFilterSelection = 2;
    private readonly List<IngredientsData.Ingredient> selectionFilter = new List<IngredientsData.Ingredient>();

    private int totalPages = 120;
    public int TotalPages => totalPages;
    public event Action OnContentChanged;

    private void Start()
    {
        filters?[0].Enable();
    }

    private void OnEnable()
    {
        
        filters[0].SetTooltip(leafs);
        filters[1].SetTooltip(mushrooms);
        filters[2].SetTooltip(roots);
        filters[3].SetTooltip(animals);
        
        GenerateData();
        
        foreach (IngredientTypeFilter filter in filters)
        {
            filter.AddedFilter += OnUpdateButtonFilter;
            filter.Show += OnUpdateFilter;
        }
    }

    private void OnDisable()
    {
        foreach (IngredientTypeFilter filter in filters)
        {
            filter.AddedFilter -= OnUpdateButtonFilter;
            filter.Show -= OnUpdateFilter;
        }
    }

    public void RecordAttempt(WrongPotion mix)
    {
        if (wrongPotions is null)
        {
            wrongPotions = new List<WrongPotion>(10);
        }

        if (!wrongPotions.Any(wrongRecipe => wrongRecipe.IngredientsList.All(mix.IngredientsList.Contains)))
        {
            wrongPotions.Add(mix);
        }
    }

    public void UpdateTab(int page)
    {
        DisplayData(page);
    }

    private void DisplayData(int page)
    {
        for (int i = 0; i < attemptEntries.Count; i++)
        {
            int count = i + page * attemptEntries.Count;
            if (count >= currentRecipes.Count)
            {
                attemptEntries[i].Clear();
                continue;
            }
            var set = currentRecipes[count];

            if (set.SetType == IngredientSetType.Recipe)
            {
                attemptEntries[i].DisplayPotion(set.Ingredients, set.Recipe);
            }
            else if (set.SetType == IngredientSetType.Failure)
            {
                attemptEntries[i].DisplayFailure(set.Ingredients);
            }
            else
            {
                attemptEntries[i].DisplayNotTried(set.Ingredients);
            }
        }
    }

    public void GenerateData()
    {
        currentRecipes = new List<IngredientSet>();
        var ingredientsList = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>().ToList();
        for (int i = 0; i < ingredientsList.Count; i++)
        {
            if (selectionFilter.Count == 2)
            {
                if ((ingredientsList[i] == selectionFilter[0].type) || (ingredientsList[i] == selectionFilter[1].type))
                {
                    continue;
                }
                AddIngredientSet(selectionFilter[0].type, selectionFilter[1].type, ingredientsList[i]);
                continue;
            }
            for (int j = i + 1; j < ingredientsList.Count; j++)
            {
                if (selectionFilter.Count == 1)
                {
                    if ((ingredientsList[i] == selectionFilter[0].type) || (ingredientsList[j] == selectionFilter[0].type))
                    {
                        continue;
                    }
                    AddIngredientSet(selectionFilter[0].type, ingredientsList[i], ingredientsList[j]);
                    continue;
                }
                for (int k = j + 1; k < ingredientsList.Count; k++)
                {
                    AddIngredientSet(ingredientsList[i], ingredientsList[j], ingredientsList[k]);
                }
            }
        }
        currentRecipes.Sort();
        totalPages = currentRecipes.Count / attemptEntries.Count + 1;
    }

    private void AddIngredientSet(Ingredients i1, Ingredients i2, Ingredients i3)
    {
        var ingredientSet = new IngredientSet
        {
            Ingredients = new[] {i1, i2, i3}
        };
        if (TryFindWrongRecipe(ingredientSet.Ingredients, out _))
        {
            ingredientSet.SetType = IngredientSetType.Failure;
        }
        else if (TryFindRecipe(ingredientSet.Ingredients, out var recipe))
        {
            ingredientSet.SetType = IngredientSetType.Recipe;
            ingredientSet.Recipe = recipe;
        }
        currentRecipes.Add(ingredientSet);
    }

    private bool TryFindWrongRecipe(Ingredients[] set, out WrongPotion result)
    {
        foreach (WrongPotion wrongPotion in wrongPotions)
        {
            bool ingredient = wrongPotion.SearchRecipe(set[0],
                set[1], set[2]);

            if (ingredient)
            {
                result = wrongPotion;
                return true;
            }
        }
        result = null;
        return false;
    }

    private bool TryFindRecipe(Ingredients[] set, out Recipe recipe)
    {
        foreach (Recipe entry in recipeBook.UnlockedRecipes)
        {
            bool ingredient = entry.SearchRecipe(set[0],
                set[1], set[2]);

            if (ingredient)
            {
                recipe = entry;
                return true;
            }
        }
        recipe = null;
        return false;
    }

    private void OnUpdateFilter(IngredientsData.Ingredient ingredient)
    {
        if (selectionFilter.Contains(ingredient))
        {
            selectionFilter.Remove(ingredient);
        }
        else
        {
            if (selectionFilter.Count == MaxFilterSelection)
            {
                selectionFilter.Clear();

                foreach (IngredientTypeFilter filter in filters)
                {
                    filter.ClearFilter(ingredient);
                }
            }
            selectionFilter.Add(ingredient);
        }

        GenerateData();
        OnContentChanged?.Invoke();
    }

    private void OnUpdateButtonFilter()
    {
        foreach (IngredientTypeFilter filter in filters)
        {
            if (filter.IsEnable)
            {
                filter.DisableButton();
            }
        }
    }
}