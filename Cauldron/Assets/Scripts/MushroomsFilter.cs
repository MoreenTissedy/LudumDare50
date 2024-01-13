using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;

public class MushroomsFilter : MonoBehaviour
{
    [SerializeField] private Button mushroomsTypeButton;
    [SerializeField] private FilterButton amanitaButton;
    [SerializeField] private FilterButton toadstoolButton;
    [SerializeField] private FilterButton agaricusButton;
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
        mushroomsTypeButton.onClick.AddListener(EnableButtons);
        amanitaButton.SwitchFilter += OnSwitchFilter;
        toadstoolButton.SwitchFilter += OnSwitchFilter;
        agaricusButton.SwitchFilter += OnSwitchFilter;
    }

    private void OnDisable()
    {
        mushroomsTypeButton.onClick.RemoveListener(EnableButtons);
        amanitaButton.SwitchFilter -= OnSwitchFilter;
        toadstoolButton.SwitchFilter -= OnSwitchFilter;
        agaricusButton.SwitchFilter -= OnSwitchFilter;
    }

    public void ClearFilter(IngredientsData.Ingredient lastIngredient)
    {
        gauge.gameObject.SetActive(false);
        amanitaButton.DisableFilter();
        toadstoolButton.DisableFilter();
        agaricusButton.DisableFilter();
        
        if (amanitaButton.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            amanitaButton.EnableDisableFilter();
        }
        else if (toadstoolButton.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            toadstoolButton.EnableDisableFilter();
        }
        else if (agaricusButton.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            agaricusButton.EnableDisableFilter();
        }
    }

    public void DisableButton()
    {
        amanitaButton.gameObject.SetActive(false);
        toadstoolButton.gameObject.SetActive(false);
        agaricusButton.gameObject.SetActive(false);
        IsShow = false;
    }

    private void EnableButtons()
    {
        AddedFilter?.Invoke();
        amanitaButton.gameObject.SetActive(true);
        toadstoolButton.gameObject.SetActive(true);
        agaricusButton.gameObject.SetActive(true);
        IsShow = true;
    }

    private void OnSwitchFilter(IngredientsData.Ingredient ingredient)
    {
        Show?.Invoke(ingredient);
        gauge.gameObject.SetActive(true);
    }
}