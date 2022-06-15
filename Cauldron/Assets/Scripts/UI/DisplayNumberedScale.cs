using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Zenject;

namespace CauldronCodebase
{
    public class DisplayNumberedScale : MonoBehaviour
    {
        private Text text;
        [Inject]
        GameManager gm;
        [Inject] private MainSettings settings;

        private void Start()
        {
            text = GetComponent<Text>();
            UpdateMoney();
            gm.GameState.statusChanged += UpdateMoney;
        }

        private void UpdateMoney()
        {
            text.text = $"{gm.GameState.Money} / {settings.gameplay.statusBarsMax}";
        }
    
    }
}