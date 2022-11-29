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
        private void Construct(MainSettings mainSettings, GameData gameData)
        {
            settings = mainSettings;
            this.gameData = gameData;
            text = GetComponent<Text>();
            UpdateMoney();
            this.gameData.StatusChanged += UpdateMoney;
        }

        private void UpdateMoney()
        {
            text.text = $"{gameData.Money} / {settings.statusBars.Total}";
        }
    
    }
}