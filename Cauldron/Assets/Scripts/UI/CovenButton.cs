using UnityEngine.EventSystems;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class CovenButton : GrowOnMouseEnter
    {
        public Statustype type;
        public bool up;

        private GameDataHandler gameDataHandler;
        
        [Inject]
        private void Construct(GameDataHandler dataHandler, MainSettings settings)
        {
            gameDataHandler = dataHandler;
        }
        
        //TODO: reactive enabling and disabling
        //TODO: onpointerenter hints

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            gameDataHandler.ChangeStatusRequest(type, up);
        }
    }
}