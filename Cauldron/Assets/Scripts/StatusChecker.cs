using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class StatusChecker
    {
        private readonly GameDataHandler gameDataHandler;
        private readonly PriorityLaneProvider cardProvider;
        private readonly MainSettings settings;

        private bool priorityCardSelected;

        public StatusChecker(MainSettings settings,
                             GameDataHandler gameDataHandler, PriorityLaneProvider cardProvider, GameStateMachine gameStateMachine)
        {
            this.settings = settings;
            this.gameDataHandler = gameDataHandler;
            this.cardProvider = cardProvider;
            gameStateMachine.OnChangeState += GameStateMachineOnChangeState;
        }

        private void GameStateMachineOnChangeState(GameStateMachine.GamePhase phase)
        {
            if (phase == GameStateMachine.GamePhase.Night)
            {
                priorityCardSelected = false;
            }
        }

        public EndingsProvider.Unlocks Run()
        {
            if (gameDataHandler.Fame >= settings.statusBars.Total)
            {
                return EndingsProvider.Unlocks.HighFame;
            }

            if (gameDataHandler.Fear >= settings.statusBars.Total)
            {
                return EndingsProvider.Unlocks.HighFear;
            }

            if (gameDataHandler.Money >= settings.statusBars.Total)
            {
                return EndingsProvider.Unlocks.HighMoney;

            }

            if (gameDataHandler.Fame <= 0)
            {
                return EndingsProvider.Unlocks.LowFame;
            }

            if (gameDataHandler.Fear <= 0)
            {
                return EndingsProvider.Unlocks.LowFear;
            }

            return EndingsProvider.Unlocks.None;
        }

        public Encounter CheckStatusesThreshold()
        {
            if (TryGetPriorityCard(PriorityLaneProvider.HIGH_FEAR, Statustype.Fear, out var card1))
            {
                return card1;
            }
            if (TryGetPriorityCard(PriorityLaneProvider.HIGH_FAME, Statustype.Fame, out var card3))
            {
                return card3;
            }
            if (TryGetPriorityCard(PriorityLaneProvider.LOW_FEAR, Statustype.Fear, out var card2, false))
            {
                return card2;
            }
            if (TryGetPriorityCard(PriorityLaneProvider.LOW_FAME, Statustype.Fame, out var card4, false))
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
                if (card != null)
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