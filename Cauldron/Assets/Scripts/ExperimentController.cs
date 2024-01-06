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

    private bool isFindWrongRecipe;

    private void OnEnable()
    {
        animalsFilter.SwitchFilter += UpdateButtonFilter;
        rootsFilter.SwitchFilter += UpdateButtonFilter;
        mushroomsFilter.SwitchFilter += UpdateButtonFilter;
        mushroomsFilter.Show += UpdateFilter;
        plantsFilter.AddedFilter += UpdateButtonFilter;
    }

    private void OnDisable()
    {
        animalsFilter.SwitchFilter -= UpdateButtonFilter;
        rootsFilter.SwitchFilter -= UpdateButtonFilter;
        mushroomsFilter.SwitchFilter -= UpdateButtonFilter;
        mushroomsFilter.Show -= UpdateFilter;
        plantsFilter.AddedFilter -= UpdateButtonFilter;
    }

    private void UpdateList(IngredientsData.Ingredient filterIngredient)
    {
        foreach (AttemptEntry attempt in attemptEntries)
        {
            Ingredients[] recipe = CreateRecipe(filterIngredient);
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

    private Ingredients[] CreateRecipe(IngredientsData.Ingredient filterIngredient)
    {
        Ingredients targetType = filterIngredient.type;
        Ingredients targetType1 = RandomIngredient(targetType);
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
        UpdateList(ingredient);
    }

    private void UpdateButtonFilter()
    {
        if(plantsFilter.IsShow)
            plantsFilter.DisableButton();
        if(animalsFilter.IsShow)
            animalsFilter.DisableButton();
        if(rootsFilter.IsShow)
            rootsFilter.DisableButton();
        if(mushroomsFilter.IsShow)
            mushroomsFilter.DisableButton();
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