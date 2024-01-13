using System;
using System.Collections.Generic;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.UI;

public class AnimalsFilter : MonoBehaviour
{
    [SerializeField] private Button animalsTypeButton;
    [SerializeField] private FilterButton batButton;
    [SerializeField] private FilterButton snakeButton;
    [SerializeField] private FilterButton ratButton;
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
        animalsTypeButton.onClick.AddListener(EnableButtons);
        batButton.SwitchFilter += OnSwitchFilter;
        snakeButton.SwitchFilter += OnSwitchFilter;
        ratButton.SwitchFilter += OnSwitchFilter;
    }

    private void OnDisable()
    {
        animalsTypeButton.onClick.RemoveListener(EnableButtons);
        batButton.SwitchFilter -= OnSwitchFilter;
        snakeButton.SwitchFilter -= OnSwitchFilter;
        ratButton.SwitchFilter -= OnSwitchFilter;
    }

    public void ClearFilter(IngredientsData.Ingredient lastIngredient)
    {
        gauge.gameObject.SetActive(false);
        batButton.DisableFilter();
        ratButton.DisableFilter();
        snakeButton.DisableFilter();
        
        if (batButton.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            batButton.EnableDisableFilter();
        }
        else if (ratButton.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            ratButton.EnableDisableFilter();
        }
        else if (snakeButton.Ingredient == lastIngredient)
        {
            gauge.gameObject.SetActive(true);
            snakeButton.EnableDisableFilter();
        }
    }

    public void DisableButton()
    {
        batButton.gameObject.SetActive(false);
        ratButton.gameObject.SetActive(false);
        snakeButton.gameObject.SetActive(false);
        IsShow = false;
    }

    private void EnableButtons()
    {
        AddedFilter?.Invoke();
        batButton.gameObject.SetActive(true);
        ratButton.gameObject.SetActive(true);
        snakeButton.gameObject.SetActive(true);
        IsShow = true;
    }

    private void OnSwitchFilter(IngredientsData.Ingredient ingredient)
    {
        Show?.Invoke(ingredient);
        gauge.gameObject.SetActive(true);
    }
}