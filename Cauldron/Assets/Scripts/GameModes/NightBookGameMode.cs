using UnityEngine;
using Zenject;
using static CauldronCodebase.GameStates.GameStateMachine;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/NightBook")]
    public class NightBookGameMode : PrestigeGameMode
    {
        private GamePhase lastPhase;
        
        protected override string achievIdents => AchievIdents.GOLD_DAYS;

        private RecipeBook book;
        private NightPanel nightPanel;

        [Inject]
        public void Construct(RecipeBook book, NightPanel nightPanel)
        {
            this.book = book;
            this.nightPanel = nightPanel;
        }

        protected override void OnApply()
        {
            lastPhase = GamePhase.Night;

            var canvasNightPanel = nightPanel.gameObject.GetComponent<Canvas>();
            var bookNightPanel = book.gameObject.GetComponentInChildren<Canvas>();

            bookNightPanel.sortingLayerName = canvasNightPanel.sortingLayerName;
            bookNightPanel.sortingOrder = canvasNightPanel.sortingOrder + 1;

            book.hudButton.ChangeLayer(canvasNightPanel.sortingLayerName, canvasNightPanel.sortingOrder + 1);
            
            gameStates.OnChangeState += SetAvailableBook;

            SetAvailableBook(gameStates.currentGamePhase);
        }
        
        private void SetAvailableBook(GamePhase phase)
        {
            if (phase == GamePhase.Night)
            {
                book.hudButton.ChangeBookAvailable(true);
            }
            else if (lastPhase == GamePhase.Night && phase != GamePhase.Night)
            {
                book.hudButton.ChangeBookAvailable(false);
                book.CloseBook();
            }
            lastPhase = phase;
        }
    }
}