using Save;
using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class VisitorState : BaseGameState
    {
        private readonly EncounterDeckBase cardDeck;
        private readonly DataPersistenceManager dataPersistenceManager;
        private readonly GameDataHandler gameDataHandler;
        private readonly VisitorManager visitorManager;
        private readonly Cauldron cauldron;
        private readonly GameStateMachine stateMachine;
        private readonly NightEventProvider nightEvents;

        private readonly CardResolver resolver;

        public VisitorState(EncounterDeckBase deck,
                            MainSettings settings,
                            DataPersistenceManager dataPersistenceManager,
                            GameDataHandler gameDataHandler,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            GameStateMachine stateMachine,
                            NightEventProvider nightEventProvider)
        {
            cardDeck = deck;
            this.dataPersistenceManager = dataPersistenceManager;
            this.gameDataHandler = gameDataHandler;
            this.visitorManager = visitorManager;
            this.visitorManager.VisitorLeft += VisitorLeft;
            this.cauldron = cauldron;
            this.stateMachine = stateMachine;
            nightEvents = nightEventProvider;

            resolver = new CardResolver(settings, gameDataHandler, deck, nightEvents);
        }
        
        public override void Enter()
        {
            Encounter currentCard;

            void NewCard()
            {
                currentCard = cardDeck.GetTopCard();
                gameDataHandler.currentCard = currentCard;
            }
            
            if (gameDataHandler.loadIgnoreSaveFile == false)
            {
                if (gameDataHandler.currentCard is null)
                {
                    NewCard();
                }
                else
                {
                    currentCard = gameDataHandler.currentCard;
                }

                gameDataHandler.loadIgnoreSaveFile = true;
            }
            else
            {
                NewCard();
            }
            
            //in case we run out of cards
            if (gameDataHandler.currentCard is null)
            {
                stateMachine.SwitchState(GameStateMachine.GamePhase.EndGame);
                return;
            }
            
            currentCard.Init(gameDataHandler, cardDeck, nightEvents);           
            visitorManager.Enter(currentCard);
            cauldron.PotionBrewed += EndEncounter;
            dataPersistenceManager.SaveGame();
        }
        
        public override void Exit()
        {
            gameDataHandler.cardsDrawnToday++;
            cauldron.PotionBrewed -= EndEncounter;
            visitorManager.Exit();           
        }

        private void EndEncounter(Potions potion)
        {
            Debug.Log("end encounter with "+potion);
            
            if (!resolver.EndEncounter(potion))
            {
                gameDataHandler.AddPotion(potion, true);
            }
            else
            {
                gameDataHandler.AddPotion(potion, false);
            }

            stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }

        private void VisitorLeft()
        {
            stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }
    }
}