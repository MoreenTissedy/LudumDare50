using System;
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
        return SetType.CompareTo(other.SetType);
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
    private const int ZeroFilter = 0;
    private const int FirstFilter = 1;
    private const int SecondFilter = 2;
    
    private IngredientsData.Ingredient mainIngredient;
    private IngredientsData.Ingredient secondIngredient;
    private int countFilter;

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
            if (mainIngredient != null && secondIngredient != null)
            {
                if ((ingredientsList[i] == mainIngredient.type) || (ingredientsList[i] == secondIngredient.type))
                {
                    continue;
                }
                AddIngredientSet(mainIngredient.type, secondIngredient.type, ingredientsList[i]);
                continue;
            }
            for (int j = 0; j < ingredientsList.Count; j++)
            {
                if (j == i)
                {
                    continue;
                }
                if (mainIngredient != null)
                {
                    if ((ingredientsList[i] == mainIngredient.type) || (ingredientsList[j] == mainIngredient.type))
                    {
                        continue;
                    }
                    AddIngredientSet(mainIngredient.type, ingredientsList[i], ingredientsList[j]);
                    continue;
                }
                for (int k = 0; k < ingredientsList.Count; k++)
                {
                    if (k == i || k == j)
                    {
                        continue;
                    }
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
        foreach (var currentRecipe in currentRecipes)
        {
            if (currentRecipe.Equals(ingredientSet))
            {
                return;
            }
        }
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
        if (ingredient != mainIngredient)
        {
            countFilter++;
        }
        else
        {
            if (countFilter > 0)
            {
                countFilter--;
            }
        }
        
        switch (countFilter)
        {
            case ZeroFilter:
            case FirstFilter:
                mainIngredient = ingredient;
                break;
            case SecondFilter:
                secondIngredient = ingredient;
                break;
        }
        
        if (countFilter > MaxFilterSelection)
        {
            countFilter = 0;
            plantsFilter.ClearFilter(ingredient);
            animalsFilter.ClearFilter(ingredient);
            rootsFilter.ClearFilter(ingredient);
            mushroomsFilter.ClearFilter(ingredient);
            secondIngredient = null;
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