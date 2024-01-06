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

    public IngredientsData.Ingredient AmanitaIngredient => amanitaButton.Ingredient;
    public IngredientsData.Ingredient ToadstoolIngredient => toadstoolButton.Ingredient;
    public IngredientsData.Ingredient AgaricusIngredient => agaricusButton.Ingredient;
    public bool IsEnableAmanita => amanitaButton.IsEnable;
    public bool IsEnableToadstool => toadstoolButton.IsEnable;
    public bool IsEnableAgaricus => agaricusButton.IsEnable;
    public bool IsShow { get; private set; }
    public event Action SwitchFilter;
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

    private void OnSwitchFilter(IngredientsData.Ingredient ingredient)
    {
        Show?.Invoke(ingredient);
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
        SwitchFilter?.Invoke();
        amanitaButton.gameObject.SetActive(true);
        toadstoolButton.gameObject.SetActive(true);
        agaricusButton.gameObject.SetActive(true);
        IsShow = true;
    }
}