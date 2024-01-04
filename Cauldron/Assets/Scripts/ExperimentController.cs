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
    
    public List<WrongPotion> wrongPotions;

    private void OnEnable()
    {
        animalsFilter.SwitchFilter += UpdateFilter;
        rootsFilter.SwitchFilter += UpdateFilter;
        mushroomsFilter.SwitchFilter += UpdateFilter;
        plantsFilter.SwitchFilter += UpdateFilter;
    }

    private void OnDisable()
    {
        animalsFilter.SwitchFilter -= UpdateFilter;
        rootsFilter.SwitchFilter -= UpdateFilter;
        mushroomsFilter.SwitchFilter -= UpdateFilter;
        plantsFilter.SwitchFilter -= UpdateFilter;
    }

    private void UpdateFilter()
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