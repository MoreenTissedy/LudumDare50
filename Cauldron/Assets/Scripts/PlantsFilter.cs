using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;

public class PlantsFilter : MonoBehaviour
{
    [SerializeField] private Button plantsTypeButton;
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
        plantsTypeButton.onClick.AddListener(EnableButtons);
        leaf1Button.SwitchFilter += OnSwitchFilter;
        leaf2Button.SwitchFilter += OnSwitchFilter;
    }

    private void OnDisable()
    {
        plantsTypeButton.onClick.RemoveListener(EnableButtons);
        leaf1Button.SwitchFilter -= OnSwitchFilter;
        leaf2Button.SwitchFilter -= OnSwitchFilter;
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
        Show?.Invoke(ingredient);
        gauge.gameObject.SetActive(true);
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
}