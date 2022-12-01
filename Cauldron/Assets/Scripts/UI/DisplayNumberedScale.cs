using CauldronCodebase.GameStates;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class DisplayNumberedScale : MonoBehaviour
    {
        private Text text;
        
        private GameData gameData;
        private MainSettings settings;

        [Inject]
        private void Construct(MainSettings mainSettings, GameData data)
        {
            settings = mainSettings;
            gameData = data;
            text = GetComponent<Text>();
            UpdateMoney();
            gameData.StatusChanged += UpdateMoney;
        }

        private void UpdateMoney()
        {
            text.text = $"{gameData.Money} / {settings.statusBars.Total}";
        }
    
    }
}