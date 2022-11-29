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
        private StateMachine _stateMachine;
        

        public event Action<int, int> NewEncounter;

        public VisitorState(EncounterDeckBase deck,
                            MainSettings settings,
                            GameData gameData,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            StateMachine stateMachine)
        {
            _cardDeck = deck;
            _mainSettings = settings;
            this.gameData = gameData;
            _visitorManager = visitorManager;
            _visitorManager.VisitorLeft += Exit;
            _cauldron = cauldron;
            _stateMachine = stateMachine;
        }
        
        public override void Enter()
        {
            Encounter currentCard = _cardDeck.GetTopCard();
            gameData.currentCard = currentCard;
            //in case we run out of cards
            if (gameData.currentCard is null)
            {
                _stateMachine.SwitchState(_stateMachine.EndGameState, false);
                return;
            }
            NewEncounter?.Invoke(gameData.cardsDrawnToday, _mainSettings.gameplay.cardsPerDay);
            
            currentCard.Init(gameData, _cardDeck, _stateMachine.NightEvents);
            gameData.cardsDrawnToday ++;
            _visitorManager.Enter(currentCard);
            _cauldron.PotionBrewed += EndEncounter;
        }
        
        public override void Exit()
        {
            _stateMachine.SwitchState(_stateMachine.VisitorWaitingState, false);
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