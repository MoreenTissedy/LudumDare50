using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExperimentController : MonoBehaviour
{
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private AnimalsFilter animalsFilter;
    [SerializeField] private RootsFilter rootsFilter;
    [SerializeField] private MushroomsFilter mushroomsFilter;
    [SerializeField] private PlantsFilter plantsFilter;

    public List<AttemptEntry> attemptEntries;
    public List<WrongPotion> wrongPotions;

    private const int MaxFilterSelection = 2;
    private const int FirstFilter = 1;
    private const int SecondFilter = 2;

    private IngredientsData.Ingredient firstIngredient;
    private IngredientsData.Ingredient secondIngredient;
    private bool isFindWrongRecipe;
    private int countFilter;

    private void OnEnable()
    {
        animalsFilter.AddedFilter += UpdateButtonFilter;
        rootsFilter.AddedFilter += UpdateButtonFilter;
        mushroomsFilter.AddedFilter += UpdateButtonFilter;
        plantsFilter.AddedFilter += UpdateButtonFilter;
        animalsFilter.Show += UpdateFilter;
        rootsFilter.Show += UpdateFilter;
        mushroomsFilter.Show += UpdateFilter;
        plantsFilter.Show += UpdateFilter;
    }

    private void OnDisable()
    {
        animalsFilter.AddedFilter -= UpdateButtonFilter;
        rootsFilter.AddedFilter -= UpdateButtonFilter;
        mushroomsFilter.AddedFilter -= UpdateButtonFilter;
        plantsFilter.AddedFilter -= UpdateButtonFilter;
        animalsFilter.Show -= UpdateFilter;
        rootsFilter.Show -= UpdateFilter;
        mushroomsFilter.Show -= UpdateFilter;
        plantsFilter.Show -= UpdateFilter;
    }

    private void UpdateList(IngredientsData.Ingredient filterIngredient)
    {
        switch (countFilter)
        {
            case FirstFilter:
                firstIngredient = filterIngredient;
                break;
            case SecondFilter:
                secondIngredient = filterIngredient;
                break;
        }

        foreach (AttemptEntry attempt in attemptEntries)
        {
            Ingredients[] recipe = CreateRecipe(firstIngredient, secondIngredient);
            WrongPotion potionWrong = null;
            Recipe potionRecipe = null;

            foreach (WrongPotion wrongPotion in wrongPotions) 
            {
                bool ingredient = wrongPotion.SearchRecipe(recipe[0], recipe[1], recipe[2]);
                
                if (ingredient) 
                {
                    isFindWrongRecipe = true;
                    potionWrong = wrongPotion;
                    break;
                }
            }
            
            foreach (Recipe herbalRecipes in recipeBook.allHerbalRecipes)
            {
                bool ingredient = herbalRecipes.SearchRecipe(recipe[0], recipe[1], recipe[2]);
                
                if (ingredient) 
                {
                    potionRecipe = herbalRecipes;
                    break;
                }
            }

            foreach (Recipe magicalRecipe in recipeBook.allMagicalRecipes)
            {
                bool ingredient = magicalRecipe.SearchRecipe(recipe[0], recipe[1], recipe[2]);
                
                if (ingredient) 
                {
                    potionRecipe = magicalRecipe;
                    break;
                }
            }

            if (!recipeBook.LockedRecipes.Contains(potionRecipe) && potionRecipe != null)
            {
                attempt.DisplayPotion(recipe, potionRecipe);
            }
            else if (isFindWrongRecipe && potionWrong != null)
            {
                attempt.DisplayFailure(recipe);
            }
            else
            {
                attempt.DisplayNotTried(recipe);
                isFindWrongRecipe = false;
            }
        }
    }

    private Ingredients[] CreateRecipe(IngredientsData.Ingredient filterIngredient, IngredientsData.Ingredient filterIngredient1 = null)
    {
        Ingredients targetType = filterIngredient.type;
        Ingredients targetType1 = filterIngredient1?.type ?? RandomIngredient(targetType);
        Ingredients targetType2 = RandomIngredient(targetType, targetType1);
        Ingredients[] recipe = { targetType, targetType1, targetType2 };

        return recipe;
    }

    private Ingredients RandomIngredient(Ingredients targetType, Ingredients targetType1)
    {
        List<Ingredients> ingredientsList = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>()
            .Where(ingredient => ingredient != targetType && ingredient != targetType1).ToList();
        
        int randomIndex = Random.Range(0, ingredientsList.Count);
        
        return ingredientsList[randomIndex];
    }

    private Ingredients RandomIngredient(Ingredients targetType)
    {
        List<Ingredients> ingredientsList = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>()
            .Where(ingredient => ingredient != targetType).ToList();
        
        int randomIndex = Random.Range(0, ingredientsList.Count);
        
        return ingredientsList[randomIndex];
    }

    private void UpdateFilter(IngredientsData.Ingredient ingredient)
    {
        countFilter++;
        
        if (countFilter > MaxFilterSelection)
        {
            countFilter = 0;
            plantsFilter.ClearFilter(ingredient);
            animalsFilter.ClearFilter(ingredient);
            rootsFilter.ClearFilter(ingredient);
            mushroomsFilter.ClearFilter(ingredient);
            secondIngredient = null;
        }

        UpdateList(ingredient);
    }

    private void UpdateButtonFilter()
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
}