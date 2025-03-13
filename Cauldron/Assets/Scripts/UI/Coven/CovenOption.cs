using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Universal;

namespace CauldronCodebase
{
    public class CovenOption : FlexibleButton
    {
        public CovenNightEventProvider covenNightEventProvider;
        public Statustype status = Statustype.Fear;
        public bool high;
        public GameObject blackout;
        public GameObject callToAction;
        private GameDataHandler gameDataHandler;
        private NightPanel nightPanel;
        
        [Inject]
        private void Construct(GameDataHandler dataHandler, NightPanel panel)
        {
            gameDataHandler = dataHandler;
            nightPanel = panel;
        }

        private void OnEnable()
        {
            if (!gameDataHandler)
            {
                return;
            }
            blackout.SetActive(!gameDataHandler.IsEnoughMoneyForRumours());
            callToAction.SetActive(gameDataHandler.IsEnoughMoneyForRumours());
            IsInteractive = gameDataHandler.IsEnoughMoneyForRumours();
        }

        public override void Activate()
        {
            base.Activate();
            
            gameDataHandler.BuyRumour();
            nightPanel.AddEventAsLast(covenNightEventProvider.GetRandom(status, high)).Forget();
        }
    }
}