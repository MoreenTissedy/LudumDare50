namespace CauldronCodebase.GameStates
{
    public class VisitorWaitingState : BaseGameState
    {
        private MainSettings _settings;
        private GameData gameData;
        private StateMachine _stateMachine;

        public VisitorWaitingState(MainSettings settings, GameData gameData, StateMachine stateMachine)
        {
            _settings = settings;
            this.gameData = gameData;
            _stateMachine = stateMachine;
        }
        
        public override void Enter()
        {
            Exit();
        }

        public override void Exit()
        {
            if (gameData.cardsDrawnToday >= _settings.gameplay.cardsPerDay)
            {
                _stateMachine.SwitchState(_stateMachine.NightState, true);
                
            }
            else
            {
                _stateMachine.SwitchState(_stateMachine.VisitorState, true);
            }
        }
    }
}