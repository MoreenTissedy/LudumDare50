using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;
using Universal;

public class FilterButton : MonoBehaviour
{
    [SerializeField] private Ingredients ingredientsName;
    [SerializeField] private FlexibleButton button;
    [SerializeField] private Image yellowBackground;
    [SerializeField] private IngredientsData ingredientsData;

    public IngredientsData.Ingredient Ingredient { get; private set; }
    public bool IsEnable { get; set; }

    public event Action<IngredientsData.Ingredient> SwitchFilter;

    private void OnEnable()
    {
        button.OnClick += ToggleFilter;
    }

    private void OnDisable()
    {
        button.OnClick -= ToggleFilter;
    }

    private void ToggleFilter()
    {
        if (!IsEnable)
        {
            EnableFilter();
        }
        else
        {
            DisableFilter();
        }
        SwitchFilter?.Invoke(Ingredient);
    }

    public void EnableFilter()
    {
        yellowBackground.gameObject.SetActive(true);
        IngredientsData.Ingredient ingredient = ingredientsData.Get(ingredientsName);
        Ingredient = ingredient;
        IsEnable = true;
    }

    public void DisableFilter()
    {
        yellowBackground.gameObject.SetActive(false);
        IsEnable = false;
    }
}