using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;
using Universal;

public class FilterButton : MonoBehaviour
{
    [SerializeField] private Ingredients ingredientsName;
    [SerializeField] private AnimatedButton button;
    [SerializeField] private Image yellowBackground;
    [SerializeField] private IngredientsData ingredientsData;

    public IngredientsData.Ingredient Ingredient { get; private set; }
    public bool IsEnable { get; set; }

    public event Action<IngredientsData.Ingredient> SwitchFilter;

    private void OnEnable()
    {
        button.OnClick += EnableDisableFilter;
    }

    private void OnDisable()
    {
        button.OnClick -= EnableDisableFilter;
    }

    public void EnableDisableFilter()
    {
        if (!IsEnable)
        {
            yellowBackground.gameObject.SetActive(true);
            IngredientsData.Ingredient ingredient = ingredientsData.Get(ingredientsName);
            Ingredient = ingredient;
            IsEnable = true;
        }
        else
        {
            DisableFilter();
        }
        
        SwitchFilter?.Invoke(Ingredient);
    }

    public void DisableFilter()
    {
        yellowBackground.gameObject.SetActive(false);
        IsEnable = false;
    }
}