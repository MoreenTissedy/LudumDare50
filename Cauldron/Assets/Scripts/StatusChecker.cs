using System.Collections.Generic;

namespace CauldronCodebase
{
    public class StatusChecker
    {
        private readonly GameDataHandler gameDataHandler;
        private readonly PriorityLaneProvider cardProvider;
        private readonly MainSettings settings;

        public StatusChecker(MainSettings settings,
                             GameDataHandler gameDataHandler, PriorityLaneProvider cardProvider)
        {
            this.settings = settings;
            this.gameDataHandler = gameDataHandler;
            this.cardProvider = cardProvider;
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

        public IEnumerable<Encounter> CheckStatusesThreshold()
        {
            yield return GetPriorityCard(PriorityLaneProvider.HIGH_FEAR, Statustype.Fear);
            yield return GetPriorityCard(PriorityLaneProvider.LOW_FEAR, Statustype.Fear, false);
            yield return GetPriorityCard(PriorityLaneProvider.HIGH_FAME, Statustype.Fame);
            yield return GetPriorityCard(PriorityLaneProvider.LOW_FAME, Statustype.Fame, false);
        }

        private Encounter GetPriorityCard(string tag, Statustype type, bool checkHigh = true)
        {
            if (CheckThreshold(type, checkHigh))
            {
                return cardProvider.GetRandomCard(tag);
            }
            return null;
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