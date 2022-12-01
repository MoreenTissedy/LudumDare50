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
        

        public event Action<int, int> NewEncounter;

        public VisitorState(EncounterDeckBase deck,
                            MainSettings settings,
                            GameData gameData,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            GameStateMachine stateMachine,
                            NightEventProvider nightEventProvider,
                            TimeBar timeBar)
        {
            _cardDeck = deck;
            _mainSettings = settings;
            this.gameData = gameData;
            _visitorManager = visitorManager;
            _visitorManager.VisitorLeft += Exit;
            _cauldron = cauldron;
            cauldron.visitorState = this;
            _stateMachine = stateMachine;
            nightEvents = nightEventProvider;
            timeBar.visitorState = this;
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
            NewEncounter?.Invoke(gameData.cardsDrawnToday, _mainSettings.gameplay.cardsPerDay);
            
            currentCard.Init(gameData, _cardDeck, nightEvents);
            gameData.cardsDrawnToday ++;
            _visitorManager.Enter(currentCard);
            _cauldron.PotionBrewed += EndEncounter;
        }
        
        private void Exit()
        {
            _stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }

        private void EndEncounter(Potions potion)
        {
            Debug.Log("end encounter with "+potion);
            _cauldron.PotionBrewed -= EndEncounter;
            _visitorManager.Exit();
            
            gameData.potionsTotal.Add(potion);

            if (!gameData.currentCard.EndEncounter(potion, _mainSettings))
            {
                gameData.wrongPotionsCount++;
            }
            
            Exit();
        }
    }
}