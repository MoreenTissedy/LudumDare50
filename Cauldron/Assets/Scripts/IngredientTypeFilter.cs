using System;
using System.Linq;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;
using Universal;

public class IngredientTypeFilter : MonoBehaviour
{
    [SerializeField] private AnimatedButton plantsTypeButton;
    [SerializeField] private FilterButton[] buttons;
    [SerializeField] private Image gauge;
    [SerializeField] private Image selection;
    
    public bool IsShow { get; private set; }
    
    public event Action AddedFilter;
    public event Action<IngredientsData.Ingredient> Show;
    
    private void Awake()
    {
        DisableButton();
    }

    private void OnEnable()
    {
        plantsTypeButton.OnClick += EnableButtons;
        foreach (var filterButton in buttons)
        {
            filterButton.SwitchFilter += OnSwitchFilter;
        }
    }

    private void OnDisable()
    {
        plantsTypeButton.OnClick -= EnableButtons;
        foreach (var filterButton in buttons)
        {
            filterButton.SwitchFilter -= OnSwitchFilter;
        }
    }

    public void ClearFilter(IngredientsData.Ingredient lastIngredient)
    {
        gauge.gameObject.SetActive(false);
        foreach (var filterButton in buttons)
        {
            filterButton.DisableFilter();
            if (filterButton.Ingredient == lastIngredient)
            {
                gauge.gameObject.SetActive(filterButton.Ingredient == lastIngredient);
                filterButton.EnableDisableFilter();
            }
        }
    }

    public void DisableButton()
    {
        foreach (var filterButton in buttons)
        {
            filterButton.gameObject.SetActive(false);
        }
        IsShow = false;
        selection.gameObject.SetActive(false);
    }

    private void EnableButtons()
    {
        AddedFilter?.Invoke();
        foreach (var filterButton in buttons)
        {
            filterButton.gameObject.SetActive(true);
        }
        IsShow = true;
        selection.gameObject.SetActive(true);
    }

    private void OnSwitchFilter(IngredientsData.Ingredient ingredient)
    {
        gauge.gameObject.SetActive(buttons.Any(x => x.IsEnable));
        Show?.Invoke(ingredient);
    }
}