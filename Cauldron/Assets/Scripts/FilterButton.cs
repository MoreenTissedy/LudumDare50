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
    public bool IsEnable { get; private set; }

    public event Action SwitchFilter;

    private void OnEnable()
    {
        button.onClick.AddListener(EnableDisableFilter);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(EnableDisableFilter);
    }

    private void EnableDisableFilter()
    {
        if (!IsEnable)
        {
            yellowBackground.gameObject.SetActive(true);
            IngredientsData.Ingredient ingredient = ingredientsData.Get(ingredientsName);
            Ingredient = ingredient;
            IsEnable = true;
            SwitchFilter?.Invoke();
        }
        else
        {
            yellowBackground.gameObject.SetActive(false);
            IsEnable = false;
        }
    }
}