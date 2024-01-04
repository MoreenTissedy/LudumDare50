using System;
using UnityEngine;
using UnityEngine.UI;

public class RootsFilter : MonoBehaviour
{
    [SerializeField] private Button rootTypeButton;
    [SerializeField] private FilterButton root1Button;
    [SerializeField] private FilterButton root2Button;
    [SerializeField] private Image gauge;

    public bool IsShow { get; private set; }
    
    public event Action SwitchFilter;

    private void Awake()
    {
        DisableButton();
    }
    
    private void OnEnable()
    {
        rootTypeButton.onClick.AddListener(EnableButtons);
    }

    private void OnDisable()
    {
        rootTypeButton.onClick.RemoveListener(EnableButtons);
    }

    public void DisableButton()
    {
        root1Button.gameObject.SetActive(false);
        root2Button.gameObject.SetActive(false);
        IsShow = false;
    }

    private void EnableButtons()
    {
        SwitchFilter?.Invoke();
        root1Button.gameObject.SetActive(true);
        root2Button.gameObject.SetActive(true);
        IsShow = true;
    }
}