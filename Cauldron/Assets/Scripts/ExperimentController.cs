using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ExperimentController : MonoBehaviour
{
    [SerializeField] private RecipeBook recipeBook;
    [SerializeField] private AnimalsFilter animalsFilter;
    [SerializeField] private RootsFilter rootsFilter;
    [SerializeField] private MushroomsFilter mushroomsFilter;
    [SerializeField] private PlantsFilter plantsFilter;
    [SerializeField] private Button nextPage;
    [SerializeField] private Button prevPage;

    public List<Ingredients[]> currentRecipes = new List<Ingredients[]>();
    public List<AttemptEntry> attemptEntries;
    public List<WrongPotion> wrongPotions;

    private const int MaxFilterSelection = 2;
    private const int ZeroFilter = 0;
    private const int FirstFilter = 1;
    private const int SecondFilter = 2;
    
    private IngredientsData.Ingredient maiIngredient;
    private IngredientsData.Ingredient spareIngredient;
    private bool isFindWrongRecipe;
    private int countFilter;
    private int currentPage;

    public int MaxTotalPages => 5;

    private void OnEnable()
    {
        nextPage.onClick.AddListener(OnNextPage);
        prevPage.onClick.AddListener(OnPrevPage);
        
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
        nextPage.onClick.RemoveListener(OnNextPage);
        prevPage.onClick.RemoveListener(OnPrevPage);
        
        animalsFilter.AddedFilter -= OnUpdateButtonFilter;
        rootsFilter.AddedFilter -= OnUpdateButtonFilter;
        mushroomsFilter.AddedFilter -= OnUpdateButtonFilter;
        plantsFilter.AddedFilter -= OnUpdateButtonFilter;
        animalsFilter.Show -= OnUpdateFilter;
        rootsFilter.Show -= OnUpdateFilter;
        mushroomsFilter.Show -= OnUpdateFilter;
        plantsFilter.Show -= OnUpdateFilter;
    }

    public void ResetPages()
    {
        currentPage = 0;
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

    public void FillAttemptEntries()
    {
        if (maiIngredient != null)
        {
            CreateRecipes();
            return;
        }

        CreateRecipes();
    }

    private void CreateRecipes()
    {
        for (int i = 0; i < attemptEntries.Count; i++)
        {
            currentRecipes.Add(CreateRecipe(maiIngredient, spareIngredient));
            
            WrongPotion potionWrong = TryFindWrongRecipe(currentPage * attemptEntries.Count + i);
            Recipe potionRecipe = TryFindHerbalRecipe(currentPage * attemptEntries.Count + i);

            if (potionRecipe == null)
            {
                potionRecipe = TryFindMagicalRecipe(currentPage * attemptEntries.Count + i);
            }

            if (!recipeBook.LockedRecipes.Contains(potionRecipe) && potionRecipe != null)
            {
                attemptEntries[i].DisplayPotion(currentRecipes[currentPage * attemptEntries.Count + i], potionRecipe);
            }
            else if (isFindWrongRecipe && potionWrong != null)
            {
                attemptEntries[i].DisplayFailure(currentRecipes[currentPage * attemptEntries.Count + i]);
            }
            else
            {
                attemptEntries[i].DisplayNotTried(currentRecipes[currentPage * attemptEntries.Count + i]);
                isFindWrongRecipe = false;
            }
        }
    }

    private Ingredients[] CreateRecipe(IngredientsData.Ingredient filterIngredient = null, IngredientsData.Ingredient filterIngredient1 = null)
    {
        Ingredients targetType = filterIngredient?.type ?? RandomIngredient();
        Ingredients targetType1 = filterIngredient1?.type ?? RandomIngredient(targetType);
        Ingredients targetType2 = RandomIngredient(targetType, targetType1);
        return new[] { targetType, targetType1, targetType2 };
    }

    private Ingredients RandomIngredient(Ingredients? firstTargetType = null, Ingredients? secondTargetType = null)
    {
        List<Ingredients> ingredientsList = Enum.GetValues(typeof(Ingredients)).Cast<Ingredients>()
            .Where(ingredient => ingredient != firstTargetType && ingredient != secondTargetType).ToList();
        
        int randomIndex = Random.Range(0, ingredientsList.Count);
        
        return ingredientsList[randomIndex];
    }

    private WrongPotion TryFindWrongRecipe(int index)
    {
        foreach (WrongPotion wrongPotion in wrongPotions)
        {
            bool ingredient = wrongPotion.SearchRecipe(currentRecipes[index][0],
                currentRecipes[index][1], currentRecipes[index][2]);

            if (ingredient)
            {
                isFindWrongRecipe = true;
                return wrongPotion;
            }
        }

        return null;
    }

    private Recipe TryFindHerbalRecipe(int index)
    {
        foreach (Recipe herbalRecipes in recipeBook.allHerbalRecipes)
        {
            bool ingredient = herbalRecipes.SearchRecipe(currentRecipes[index][0],
                currentRecipes[index][1], currentRecipes[index][2]);

            if (ingredient)
            {
                return herbalRecipes;
            }
        }

        return null;
    }

    private Recipe TryFindMagicalRecipe(int index)
    {
        foreach (Recipe magicalRecipe in recipeBook.allMagicalRecipes)
        {
            bool ingredient = magicalRecipe.SearchRecipe(currentRecipes[index][0],
                currentRecipes[index][1], currentRecipes[index][2]);

            if (ingredient)
            {
                return magicalRecipe;
            }
        }

        return null;
    }

    private void OnNextPage()
    {
        currentPage++;

        if (currentPage >= MaxTotalPages)
        {
            nextPage.gameObject.SetActive(false);
            return;
        }
        
        CreateRecipes();
    }

    private void OnPrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
        }
        
        CreateRecipes();
    }

    private void OnUpdateFilter(IngredientsData.Ingredient ingredient)
    {
        countFilter++;
        
        switch (countFilter)
        {
            case ZeroFilter:
            case FirstFilter:
                maiIngredient = ingredient;
                break;
            case SecondFilter:
                spareIngredient = ingredient != maiIngredient ? ingredient : spareIngredient;
                break;
        }
        
        if (countFilter > MaxFilterSelection)
        {
            countFilter = 0;
            plantsFilter.ClearFilter(ingredient);
            animalsFilter.ClearFilter(ingredient);
            rootsFilter.ClearFilter(ingredient);
            mushroomsFilter.ClearFilter(ingredient);
            spareIngredient = null;
        }

        currentRecipes = new List<Ingredients[]>();
        ResetPages();
        CreateRecipes();
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