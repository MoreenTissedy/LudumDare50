using System;
using TMPro;
using UnityEngine;

public class CatTipsView : MonoBehaviour
{
    [SerializeField] private GameObject icon;
    [SerializeField] private GameObject bubble;
    [SerializeField] private TMP_Text text;

    private bool tipEnabled;

    private void Start()
    {
        HideView();
    }

    public void ChangeTipView()
    {
        icon.SetActive(!icon.activeSelf);
        bubble.SetActive(!bubble.activeSelf);
    }

    public void ShowTips(CatTips tips)
    {
        bubble.SetActive(true);
        text.text = tips.TipsText;
    }

    public void HideView()
    {
        icon.SetActive(false);
        bubble.SetActive(false);
    }
}
