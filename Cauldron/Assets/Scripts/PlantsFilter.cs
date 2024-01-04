using System;
using UnityEngine;
using UnityEngine.UI;

public class PlantsFilter : MonoBehaviour
{
    [SerializeField] private Button plantsTypeButton;
    [SerializeField] private FilterButton leaf1Button;
    [SerializeField] private FilterButton leaf2Button;
    [SerializeField] private Image gauge;
    
    public bool IsShow { get; private set; }
    public event Action SwitchFilter;
    
    private void Awake()
    {
        DisableButton();
    }

    private void OnEnable()
    {
        plantsTypeButton.onClick.AddListener(EnableButtons);
    }

    private void OnDisable()
    {
        plantsTypeButton.onClick.RemoveListener(EnableButtons);
    }

    public void DisableButton()
    {
        leaf1Button.gameObject.SetActive(false);
        leaf2Button.gameObject.SetActive(false);
        IsShow = false;
    }

    private void EnableButtons()
    {
        SwitchFilter?.Invoke();
        leaf1Button.gameObject.SetActive(true);
        leaf2Button.gameObject.SetActive(true);
        IsShow = true;
    }
}