using CauldronCodebase.GameStates;
using Zenject;
namespace CauldronCodebase
{
    public class StatusChecker
    {
        private GameData gameData;
        private MainSettings _settings;
        private StateMachine _stateMachine;

        [Inject]
        public StatusChecker(MainSettings settings,
                             StateMachine stateMachine)
        {
            _settings = settings;
            _stateMachine = stateMachine;
            gameData = stateMachine.GameData;
        }
        
        public void Run()
        {
            if (gameData.Fame >= _settings.statusBars.Total)
            {
                _stateMachine.EndGame(EndingsProvider.Unlocks.HighFame);
                return;
            }

            if (gameData.Fear >= _settings.statusBars.Total)
            {
                _stateMachine.EndGame(EndingsProvider.Unlocks.HighFear);
                return;
            }

            if (gameData.Money >= _settings.statusBars.Total)
            {
                _stateMachine.EndGame(EndingsProvider.Unlocks.HighMoney);
                return;

            }

            if (gameData.Fame <= 0)
            {
                _stateMachine.EndGame(EndingsProvider.Unlocks.LowFame);
                return;
            }

            if (gameData.Fear <= 0)
            {
                _stateMachine.EndGame(EndingsProvider.Unlocks.LowFear);
                return;
            }

            gameData.CheckStatusesThreshold();
        }
    }
}