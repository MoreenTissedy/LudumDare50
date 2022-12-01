using System;
using UnityEngine;

namespace CauldronCodebase.GameStates
{
    public class VisitorState : BaseGameState
    {
        private EncounterDeckBase _cardDeck;
        private MainSettings _mainSettings;
        private GameData gameData;
        private VisitorManager _visitorManager;
        private Cauldron _cauldron;
        private GameStateMachine _stateMachine;
        private NightEventProvider nightEvents;

        public VisitorState(EncounterDeckBase deck,
                            MainSettings settings,
                            GameData gameData,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            GameStateMachine stateMachine,
                            NightEventProvider nightEventProvider)
        {
            _cardDeck = deck;
            _mainSettings = settings;
            this.gameData = gameData;
            _visitorManager = visitorManager;
            _visitorManager.VisitorLeft += VisitorLeft;
            _cauldron = cauldron;
            _stateMachine = stateMachine;
            nightEvents = nightEventProvider;
        }
        
        public override void Enter()
        {
            Encounter currentCard = _cardDeck.GetTopCard();
            gameData.currentCard = currentCard;
            //in case we run out of cards
            if (gameData.currentCard is null)
            {
                _stateMachine.SwitchState(GameStateMachine.GamePhase.EndGame);
                return;
            }
            
            currentCard.Init(gameData, _cardDeck, nightEvents);           
            _visitorManager.Enter(currentCard);
            _cauldron.PotionBrewed += EndEncounter;
        }
        
        public override void Exit()
        {
            gameData.cardsDrawnToday++;
            _cauldron.PotionBrewed -= EndEncounter;
            _visitorManager.Exit();           
        }

        private void EndEncounter(Potions potion)
        {
            Debug.Log("end encounter with "+potion);
                                  
            gameData.potionsTotal.Add(potion);

            if (!gameData.currentCard.EndEncounter(potion, _mainSettings))
            {
                gameData.wrongPotionsCount++;
            }

            _stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }

        private void VisitorLeft()
        {
            _stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }
    }
}