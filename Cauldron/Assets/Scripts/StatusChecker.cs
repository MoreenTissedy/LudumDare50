using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;
namespace CauldronCodebase
{
    public class StatusChecker
    {
        private GameData gameData;
        private MainSettings _settings;
        private GameStateMachine _stateMachine;

        [Inject]
        public StatusChecker(MainSettings settings,
                             GameStateMachine stateMachine,
                             GameData data)
        {
            _settings = settings;
            _stateMachine = stateMachine;
            gameData = data;
        }
        
        public EndingsProvider.Unlocks Run()
        {
            if (gameData.Fame >= _settings.statusBars.Total)
            {
                return EndingsProvider.Unlocks.HighFame;
            }

            if (gameData.Fear >= _settings.statusBars.Total)
            {
                return EndingsProvider.Unlocks.HighFear;
            }

            if (gameData.Money >= _settings.statusBars.Total)
            {
                return EndingsProvider.Unlocks.HighMoney;

            }

            if (gameData.Fame <= 0)
            {
                return EndingsProvider.Unlocks.LowFame;
            }

            if (gameData.Fear <= 0)
            {
                return EndingsProvider.Unlocks.LowFear;
            }

            CheckStatusesThreshold();
            return EndingsProvider.Unlocks.None;
        }

        public void CheckStatusesThreshold()
        {
            AddHighLowTag("high fear", Statustype.Fear);
            AddHighLowTag("low fear", Statustype.Fear, false);
            AddHighLowTag("high fame", Statustype.Fame);
            AddHighLowTag("low fame", Statustype.Fame, false);
        }

        private bool CheckThreshold(Statustype type, bool checkHigh)
        {
            int currentStatus = gameData.Get(type);
            bool thresholdReached = checkHigh ? currentStatus > gameData.GetThreshold(type, true) : currentStatus < gameData.GetThreshold(type, false);
            if (thresholdReached)
            {
                gameData.ChangeThreshold(type, checkHigh);
            }
            return thresholdReached;
        }

        private void AddHighLowTag(string tag, Statustype type, bool checkHigh = true)
        {
            bool thresholdReached = CheckThreshold(type, checkHigh);
            if (thresholdReached)
            {
                gameData.AddTag(tag);
            }
            else
            {
                gameData.storyTags.Remove(tag);
            }
        }
    }
}