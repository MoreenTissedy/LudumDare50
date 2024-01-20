using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CauldronCodebase
{
    public class CovenOption : MonoBehaviour, IPointerClickHandler
    {
        public CovenNightEventProvider covenNightEventProvider;
        public Statustype status = Statustype.Fear;
        public bool high;
        public GameObject blackout;
        public GameObject callToAction;
        private GameDataHandler gameDataHandler;
        private NightPanel nightPanel;
        private bool interactable;
        
        [Inject]
        private void Construct(GameDataHandler dataHandler, NightPanel panel)
        {
            gameDataHandler = dataHandler;
            nightPanel =panel;
        }

        private void OnEnable()
        {
            blackout.SetActive(!gameDataHandler.IsEnoughMoneyForRumours());
            callToAction.SetActive(gameDataHandler.IsEnoughMoneyForRumours());
            interactable = gameDataHandler.IsEnoughMoneyForRumours();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
            {
                return;
            }
            gameDataHandler.BuyRumour();
            nightPanel.AddEventAsLast(covenNightEventProvider.GetRandom(status, high)).Forget();
        }
    }
}