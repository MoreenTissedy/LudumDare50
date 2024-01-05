using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour
{
    [SerializeField] private AnimalsFilter animalsFilter;
    [SerializeField] private RootsFilter rootsFilter;
    [SerializeField] private MushroomsFilter mushroomsFilter;
    [SerializeField] private PlantsFilter plantsFilter;

    public List<AttemptEntry> attemptEntries;
    public List<WrongPotion> wrongPotions;

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
        int counter = 0;
        
        for (int i = 0; i < wrongPotions.Count; i++)
        {
            if (wrongPotions[i].SearchIngredient(filterIngredient.type) && counter < attemptEntries.Count)
            {
                attemptEntries[i].Display(wrongPotions[counter].IngredientsList.ToArray());
                attemptEntries[counter].Display(wrongPotions[i].IngredientsList.ToArray());
                counter++;
            }
        }
    }

    private void UpdateFilter()
    {
        if (mushroomsFilter.IsEnableAgaricus)
        {
            UpdateList(mushroomsFilter.AgaricusIngredient);
        }
        else if (mushroomsFilter.IsEnableToadstool)
        {
            UpdateList(mushroomsFilter.ToadstoolIngredient);
        }
        else if (mushroomsFilter.IsEnableAmanita)
        { 
            UpdateList(mushroomsFilter.AmanitaIngredient);
        }
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