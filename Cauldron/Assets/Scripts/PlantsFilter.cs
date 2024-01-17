using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;
using Universal;

public class PlantsFilter : MonoBehaviour
{
    [SerializeField] private AnimatedButton plantsTypeButton;
    [SerializeField] private FilterButton leaf1Button;
    [SerializeField] private FilterButton leaf2Button;
    [SerializeField] private Image gauge;
    
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
        leaf1Button.SwitchFilter += OnSwitchFilter;
        leaf2Button.SwitchFilter += OnSwitchFilter;
    }

    private void OnDisable()
    {
        plantsTypeButton.OnClick -= EnableButtons;
        leaf1Button.SwitchFilter -= OnSwitchFilter;
        leaf2Button.SwitchFilter -= OnSwitchFilter;
    }

    public void ClearFilter(IngredientsData.Ingredient lastIngredient)
    {
        gauge.gameObject.SetActive(false);
        leaf1Button.DisableFilter();
        leaf2Button.DisableFilter();
        
        if (leaf1Button.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            leaf1Button.EnableDisableFilter();
        }
        else if (leaf2Button.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            leaf2Button.EnableDisableFilter();
        }
    }

    public void DisableButton()
    {
        leaf1Button.gameObject.SetActive(false);
        leaf2Button.gameObject.SetActive(false);
        IsShow = false;
    }

    private void EnableButtons()
    {
        AddedFilter?.Invoke();
        leaf1Button.gameObject.SetActive(true);
        leaf2Button.gameObject.SetActive(true);
        IsShow = true;
    }

    private void OnSwitchFilter(IngredientsData.Ingredient ingredient)
    {
        if (leaf1Button.IsEnable || leaf2Button.IsEnable)
        {
            gauge.gameObject.SetActive(true);
        }
        else
        {
            gauge.gameObject.SetActive(false);
        }
        
        Show?.Invoke(ingredient);
    }
}