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