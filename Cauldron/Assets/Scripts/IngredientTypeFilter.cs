using System;
using System.Linq;
using CauldronCodebase;
using Cysharp.Threading.Tasks;
using EasyLoc;
using UnityEngine;
using UnityEngine.UI;
using Universal;

public class IngredientTypeFilter : MonoBehaviour
{
    [Localize] public string title;

    [SerializeField] private AnimatedButton plantsTypeButton;
    [SerializeField] private FilterButton[] buttons;
    [SerializeField] private Image gauge;
    [SerializeField] private Image selection;

    private ScrollTooltip scrollTooltip;
    
    public bool IsEnable { get; private set; }
    
    public event Action AddedFilter;
    public event Action<IngredientsData.Ingredient> Show;
    
    private void Awake()
    {
        scrollTooltip = GetComponentInChildren<ScrollTooltip>();
        scrollTooltip.SetText(title).Forget();
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

    public void Enable()
    {
        EnableButtons();
    }

    public void ClearFilter(IngredientsData.Ingredient lastIngredient)
    {
        gauge.gameObject.SetActive(false);
        foreach (var filterButton in buttons)
        {
            if (filterButton.Ingredient == lastIngredient)
            {
                gauge.gameObject.SetActive(true);
                filterButton.EnableFilter();
            }
            else
            {
                filterButton.DisableFilter();
            }
        }
    }

    public void DisableButton()
    {
        foreach (var filterButton in buttons)
        {
            filterButton.gameObject.SetActive(false);
        }
        IsEnable = false;
        selection.gameObject.SetActive(false);
    }

    private void EnableButtons()
    {
        AddedFilter?.Invoke();
        foreach (var filterButton in buttons)
        {
            filterButton.gameObject.SetActive(true);
        }
        IsEnable = true;
        selection.gameObject.SetActive(true);
    }

    private void OnSwitchFilter(IngredientsData.Ingredient ingredient)
    {
        gauge.gameObject.SetActive(buttons.Any(x => x.IsEnable));
        Show?.Invoke(ingredient);
    }
}