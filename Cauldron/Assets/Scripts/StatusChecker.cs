using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class StatusChecker
    {
        private readonly GameDataHandler gameDataHandler;
        private readonly PriorityLaneProvider cardProvider;
        private readonly EncounterDeck deck;
        private readonly MainSettings settings;

        private bool priorityCardSelected;

        [Inject]
        public StatusChecker(MainSettings settings,
                             GameDataHandler gameDataHandler, PriorityLaneProvider cardProvider, GameStateMachine gameStateMachine, EncounterDeck deck)
        {
            this.settings = settings;
            this.gameDataHandler = gameDataHandler;
            this.cardProvider = cardProvider;
            this.deck = deck;
            gameStateMachine.OnChangeState += GameStateMachineOnChangeState;
        }

        private void GameStateMachineOnChangeState(GameStateMachine.GamePhase phase)
        {
            if (phase == GameStateMachine.GamePhase.Night)
            {
                priorityCardSelected = false;
            }
        }

        public string Run()
        {
            if (gameDataHandler.Fame >= settings.statusBars.Total)
            {
                return EndingsProvider.HIGH_FAME;
            }

            if (gameDataHandler.Fear >= settings.statusBars.Total)
            {
                return EndingsProvider.HIGH_FEAR;
            }

            if (gameDataHandler.Fame <= 0)
            {
                return EndingsProvider.LOW_FAME;
            }

            if (gameDataHandler.Fear <= 0)
            {
                return EndingsProvider.LOW_FEAR;
            }

            return string.Empty;
        }

        public Encounter CheckStatusesThreshold()
        {
            if (TryGetPriorityCard(EndingsProvider.HIGH_FEAR, Statustype.Fear, out var card1))
            {
                return card1;
            }
            if (TryGetPriorityCard(EndingsProvider.HIGH_FAME, Statustype.Fame, out var card3))
            {
                return card3;
            }
            if (TryGetPriorityCard(EndingsProvider.LOW_FEAR, Statustype.Fear, out var card2, false))
            {
                return card2;
            }
            if (TryGetPriorityCard(EndingsProvider.LOW_FAME, Statustype.Fame, out var card4, false))
            {
                return card4;
            }
            return null;
        }

        private bool TryGetPriorityCard(string tag, Statustype type, out Encounter card, bool checkHigh = true)
        {
            if (CheckThreshold(type, checkHigh) && !priorityCardSelected)
            {
                card = cardProvider.GetRandomCard(tag);
                if (!deck.IsCardNotValidForDeck(card))
                {
                    priorityCardSelected = true;
                    return true;
                }
            }
            card = null;
            return false;
        }

        private bool CheckThreshold(Statustype type, bool checkHigh)
        {
            int currentStatus = gameDataHandler.Get(type);
            if (gameDataHandler.ReachedMaxThreshold(type, checkHigh))
            {
                return false;
            }
            bool thresholdReached = false;
            bool nextThreshold = false;
            do
            {
                nextThreshold = checkHigh
                    ? currentStatus > gameDataHandler.GetThreshold(type, true)
                    : currentStatus < gameDataHandler.GetThreshold(type, false);
                if (nextThreshold)
                {
                    thresholdReached = true;
                }
            } while (nextThreshold && gameDataHandler.ChangeThreshold(type, checkHigh));

            return thresholdReached;
        }
    }
}