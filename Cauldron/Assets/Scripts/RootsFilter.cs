using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;
using Universal;

public class RootsFilter : MonoBehaviour
{
    [SerializeField] private AnimatedButton rootTypeButton;
    [SerializeField] private FilterButton root1Button;
    [SerializeField] private FilterButton root2Button;
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
        rootTypeButton.OnClick += EnableButtons;
        root1Button.SwitchFilter += OnSwitchFilter;
        root2Button.SwitchFilter += OnSwitchFilter;
    }

    private void OnDisable()
    {
        rootTypeButton.OnClick -= EnableButtons;
        root1Button.SwitchFilter -= OnSwitchFilter;
        root2Button.SwitchFilter -= OnSwitchFilter;
    }

    public void ClearFilter(IngredientsData.Ingredient lastIngredient)
    {
        gauge.gameObject.SetActive(false);
        root1Button.DisableFilter();
        root2Button.DisableFilter();
        
        if (root1Button.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            root1Button.EnableDisableFilter();
        }
        else if (root2Button.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            root2Button.EnableDisableFilter();
        }
    }

    public void DisableButton()
    {
        root1Button.gameObject.SetActive(false);
        root2Button.gameObject.SetActive(false);
        IsShow = false;
    }

    private void EnableButtons()
    {
        AddedFilter?.Invoke();
        root1Button.gameObject.SetActive(true);
        root2Button.gameObject.SetActive(true);
        IsShow = true;
    }

    private void OnSwitchFilter(IngredientsData.Ingredient ingredient)
    {
        if (root1Button.IsEnable || root2Button.IsEnable)
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