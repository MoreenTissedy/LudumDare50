using System;
using UnityEngine;
using UnityEngine.UI;

public class MushroomsFilter : MonoBehaviour
{
    [SerializeField] private Button mushroomsTypeButton;
    [SerializeField] private FilterButton amanitaButton;
    [SerializeField] private FilterButton toadstoolButton;
    [SerializeField] private FilterButton agaricusButton;
    [SerializeField] private Image gauge;

    public bool IsShow { get; private set; }
    public event Action SwitchFilter;

    private void Awake()
    {
        DisableButton();
    }
    
    private void OnEnable()
    {
        mushroomsTypeButton.onClick.AddListener(EnableButtons);
    }

    private void OnDisable()
    {
        mushroomsTypeButton.onClick.RemoveListener(EnableButtons);
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