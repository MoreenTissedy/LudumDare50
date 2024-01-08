using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExperimentController : MonoBehaviour
{
    [SerializeField] private AnimalsFilter animalsFilter;
    [SerializeField] private RootsFilter rootsFilter;
    [SerializeField] private MushroomsFilter mushroomsFilter;
    [SerializeField] private PlantsFilter plantsFilter;

    public List<AttemptEntry> attemptEntries;
    public List<WrongPotion> wrongPotions;

    private IngredientsData.Ingredient firstIngredient;
    private IngredientsData.Ingredient secondIngredient;
    private bool isFindWrongRecipe;
    private int countFilter;
    private const int MaxFilter = 2;

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
            case 1:
                firstIngredient = filterIngredient;
                break;
            case 2:
                secondIngredient = filterIngredient;
                break;
        }

        foreach (AttemptEntry attempt in attemptEntries)
        {
            Ingredients[] recipe = CreateRecipe(firstIngredient, secondIngredient);
            WrongPotion potion = null;

            foreach (WrongPotion wrongPotion in wrongPotions) 
            {
                bool ingredient = wrongPotion.SearchRecipe(recipe[0], recipe[1], recipe[2]);
                
                if (ingredient) 
                {
                    isFindWrongRecipe = true;
                    potion = wrongPotion;
                    break;
                }
            }

            if (isFindWrongRecipe && potion != null)
            {
                attempt.DisplayFailure(potion.IngredientsList.ToArray());
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
        
        if (countFilter > MaxFilter)
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