using UnityEngine;
using UnityEngine.UI;
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
        private ExperimentController experimentController;

        [Inject]
        public void Construct(RecipeBook book, NightPanel nightPanel, ExperimentController experimentController)
        {
            this.book = book;
            this.nightPanel = nightPanel;
            this.experimentController = experimentController;
        }

        protected override void OnApply()
        {
            lastPhase = GamePhase.Night;

            var canvasNightPanel = nightPanel.gameObject.GetComponent<Canvas>();
            var bookNightPanel = book.gameObject.GetComponentInChildren<Canvas>();

            bookNightPanel.sortingLayerName = canvasNightPanel.sortingLayerName;
            bookNightPanel.sortingOrder = canvasNightPanel.sortingOrder + 1;

            book.hudButton.ChangeLayer(canvasNightPanel.sortingLayerName, canvasNightPanel.sortingOrder + 1);
            book.isNightBook = true;
            foreach (var temp in experimentController.attemptEntries)
            {
                temp.Button.BlockButtonClick();
            }
            
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