﻿using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using NaughtyAttributes;
using UnityEngine;

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

public enum IngredientSetType
{
    Unknown,
    Recipe,
    Failure 
}

public class ExperimentController : MonoBehaviour
{
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private IngredientTypeFilter animalsFilter;
    [SerializeField] private IngredientTypeFilter rootsFilter;
    [SerializeField] private IngredientTypeFilter mushroomsFilter;
    [SerializeField] private IngredientTypeFilter plantsFilter;

    public List<IngredientSet> currentRecipes;
    [ReorderableList]
    public List<AttemptEntry> attemptEntries;
    public List<WrongPotion> wrongPotions;

    private const int MaxFilterSelection = 2;
    private readonly List<IngredientsData.Ingredient> selectionFilter = new List<IngredientsData.Ingredient>();

    private int totalPages = 120;
    public int TotalPages => totalPages;
    public event Action OnContentChanged;

    private void OnEnable()
    {
        GenerateData();
        animalsFilter.AddedFilter += OnUpdateButtonFilter;
        rootsFilter.AddedFilter += OnUpdateButtonFilter;
        mushroomsFilter.AddedFilter += OnUpdateButtonFilter;
        plantsFilter.AddedFilter += OnUpdateButtonFilter;
        animalsFilter.Show += OnUpdateFilter;
        rootsFilter.Show += OnUpdateFilter;
        mushroomsFilter.Show += OnUpdateFilter;
        plantsFilter.Show += OnUpdateFilter;
    }

    private void OnDisable()
    {
        animalsFilter.AddedFilter -= OnUpdateButtonFilter;
        rootsFilter.AddedFilter -= OnUpdateButtonFilter;
        mushroomsFilter.AddedFilter -= OnUpdateButtonFilter;
        plantsFilter.AddedFilter -= OnUpdateButtonFilter;
        animalsFilter.Show -= OnUpdateFilter;
        rootsFilter.Show -= OnUpdateFilter;
        mushroomsFilter.Show -= OnUpdateFilter;
        plantsFilter.Show -= OnUpdateFilter;
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
                plantsFilter.ClearFilter(ingredient);
                animalsFilter.ClearFilter(ingredient);
                rootsFilter.ClearFilter(ingredient);
                mushroomsFilter.ClearFilter(ingredient);
            }
            selectionFilter.Add(ingredient);
        }

        GenerateData();
        OnContentChanged?.Invoke();
    }

    private void OnUpdateButtonFilter()
    {
        if (plantsFilter.IsShow)
        {
            plantsFilter.DisableButton();
        }
        
        if (animalsFilter.IsShow)
        {
            animalsFilter.DisableButton();
        }
        
        if (rootsFilter.IsShow)
        {
            rootsFilter.DisableButton();
        }
        
        if (mushroomsFilter.IsShow)
        {
            mushroomsFilter.DisableButton();
        }
    }
}