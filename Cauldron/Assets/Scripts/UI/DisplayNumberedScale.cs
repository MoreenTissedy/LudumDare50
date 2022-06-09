using System;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class DisplayNumberedScale : MonoBehaviour
    {
        private Text text;

        private void Start()
        {
            text = GetComponent<Text>();
            UpdateMoney();
            GameManager.instance.money.changed += UpdateMoney;
        }

        private void UpdateMoney()
        {
            text.text = $"{GameManager.instance.money.Value()} / {GameManager.instance.statusBarsMax}";
        }
    
    }
}