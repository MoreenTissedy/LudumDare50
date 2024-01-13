using System;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour
{
    [SerializeField] private Ingredients ingredientsName;
    [SerializeField] private Button button;
    [SerializeField] private Image yellowBackground;
    [SerializeField] private IngredientsData ingredientsData;

    public IngredientsData.Ingredient Ingredient { get; private set; }
    private bool IsEnable { get; set; }

    public event Action<IngredientsData.Ingredient> SwitchFilter;

    private void OnEnable()
    {
        button.onClick.AddListener(EnableDisableFilter);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(EnableDisableFilter);
    }

    public void EnableDisableFilter()
    {
        if (!IsEnable)
        {
            yellowBackground.gameObject.SetActive(true);
            IngredientsData.Ingredient ingredient = ingredientsData.Get(ingredientsName);
            Ingredient = ingredient;
            IsEnable = true;
            SwitchFilter?.Invoke(Ingredient);
        }
        else
        {
            DisableFilter();
        }
    }

    public void DisableFilter()
    {
        yellowBackground.gameObject.SetActive(false);
        IsEnable = false;
    }
}