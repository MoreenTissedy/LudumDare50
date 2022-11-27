using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class DisplayNumberedScale : MonoBehaviour
    {
        private Text text;
        
        private GameManager gm;
        private MainSettings settings;

        [Inject]
        private void Construct(MainSettings mainSettings, GameManager gm)
        {
            this.gm = gm;
            settings = mainSettings;
            text = GetComponent<Text>();
            UpdateMoney();
            gm.GameState.StatusChanged += UpdateMoney;
        }

        private void UpdateMoney()
        {
            text.text = $"{gm.GameState.Money} / {settings.statusBars.Total}";
        }
    
    }
}