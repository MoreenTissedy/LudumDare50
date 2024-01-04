using System;
using System.Collections.Generic;
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
    
    public event Action SwitchFilter;

    private void Awake()
    {
        DisableButton();
    }

    private void OnEnable()
    {
        animalsTypeButton.onClick.AddListener(EnableButtons);
    }

    private void OnDisable()
    {
        animalsTypeButton.onClick.RemoveListener(EnableButtons);
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
        SwitchFilter?.Invoke();
        batButton.gameObject.SetActive(true);
        ratButton.gameObject.SetActive(true);
        snakeButton.gameObject.SetActive(true);
        IsShow = true;
    }
}