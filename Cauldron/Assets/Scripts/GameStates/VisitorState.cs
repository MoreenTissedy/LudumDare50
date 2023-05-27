using Save;
using System.Linq;
using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class VisitorState : BaseGameState
    {
        private readonly EncounterDeckBase cardDeck;
        private readonly GameDataHandler gameDataHandler;
        private readonly VisitorManager visitorManager;
        private readonly Cauldron cauldron;
        private readonly GameStateMachine stateMachine;
        private readonly SoundManager soundManager;

        private readonly EncounterResolver resolver;
        private readonly StatusChecker statusChecker;

        public VisitorState(EncounterDeckBase deck,
                            MainSettings settings,
                            GameDataHandler gameDataHandler,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            GameStateMachine stateMachine,
                            NightEventProvider nightEventProvider, 
                            SoundManager soundManager)
        {
            cardDeck = deck;
            this.gameDataHandler = gameDataHandler;
            this.visitorManager = visitorManager;
            this.visitorManager.VisitorLeft += VisitorLeft;
            this.cauldron = cauldron;
            this.stateMachine = stateMachine;
            this.soundManager = soundManager;

            statusChecker = new StatusChecker(settings, gameDataHandler);
            resolver = new EncounterResolver(settings, gameDataHandler, deck, nightEventProvider);
        }
        
        public override void Enter()
        {
            Encounter currentCard = cardDeck.GetTopCard();
            gameDataHandler.currentCard = currentCard;

            //in case we run out of cards
            if (gameDataHandler.currentCard is null)
            {
                stateMachine.currentEnding = EndingsProvider.Unlocks.HighMoney;
                stateMachine.SwitchState(GameStateMachine.GamePhase.EndGame);
                gameDataHandler.RememberRound();
                return;
            }
                     
            visitorManager.Enter(currentCard);
            cauldron.PotionAccepted += EndEncounter;
        }
        
        public override void Exit()
        {
            gameDataHandler.cardsDrawnToday++;
            cauldron.PotionAccepted -= EndEncounter;
            visitorManager.Exit();           
        }

        private void EndEncounter(Potions potion)
        {
            PlayRelevantSound(potion);
            gameDataHandler.AddPotion(potion, !resolver.EndEncounter(potion));

            statusChecker.CheckStatusesThreshold();
            cardDeck.AddStoryCards();
            stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }

        private void PlayRelevantSound(Potions potion)
        {
            var potionCoef = gameDataHandler.currentCard.resultsByPotion.FirstOrDefault(x => x.potion == potion)?.influenceCoef ?? 0;
            if (potionCoef > 0)
            {
                soundManager.Play(Sounds.PotionSuccess);
            }
            else if (potionCoef < 0)
            {
                soundManager.Play(Sounds.PotionFailure);
            }
        }

        private void VisitorLeft()
        {
            stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }
    }
}