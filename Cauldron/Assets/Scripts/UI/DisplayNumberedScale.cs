using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class DisplayNumberedScale : MonoBehaviour
    {
        private Text text;
        
        private GameDataHandler gameDataHandler;
        private MainSettings settings;

        [Inject]
        private void Construct(MainSettings mainSettings, GameDataHandler dataHandler)
        {
            settings = mainSettings;
            gameDataHandler = dataHandler;
            text = GetComponent<Text>();
            gameDataHandler.StatusChanged += UpdateMoney;
        }

        private void Start()
        {
            UpdateMoney(Statustype.Money, 0);
        }

        private void UpdateMoney(Statustype statustype, int i)
        {
            if (statustype != Statustype.Money || i == 0)
            {
                return;
            }
            if (StoryTagHelper.CovenSavingsEnabled(gameDataHandler))
            {
                text.text = $"{gameDataHandler.Money} / {settings.statusBars.CovenMoneyFee}";
            }
            else
            {
                text.text = gameDataHandler.Money.ToString();
            }
        }
    
    }
}