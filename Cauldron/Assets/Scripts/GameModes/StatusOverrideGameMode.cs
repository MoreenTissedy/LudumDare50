using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/StatusOverride")]
    public class StatusOverrideGameMode: GameModeBase
    {
        public Statustype Status;
        public bool Up;
        public bool Down;
        public int Bonus = 5;

        private GameDataHandler gameDataHandler;
        private GameStateMachine gameStateMachine;

        private bool overriden;

        [Inject]
        public void Construct(GameDataHandler gameDataHandler, GameStateMachine gameStateMachine)
        {
            this.gameDataHandler = gameDataHandler;
            this.gameStateMachine = gameStateMachine;
        }
        public override void Apply()
        {
            gameStateMachine.OnChangeState += ResetAppliedFlag;
            gameDataHandler.StatusChanged += TryOverrideStatus;
        }

        private void ResetAppliedFlag(GameStateMachine.GamePhase phase)
        {
            if (phase == GameStateMachine.GamePhase.Visitor)
            {
                overriden = false;
            }
        }

        private void TryOverrideStatus(Statustype type, int diff)
        {
            if (overriden || type != Status || diff == 0)
            {
                return;
            }

            if (diff > 0 && Up)
            {
                overriden = true;
                gameDataHandler.Add(type, GetBonus(diff));
                Debug.Log(type + " + "+GetBonus(diff));
            }

            if (diff < 0 && Down)
            {
                overriden = true;
                gameDataHandler.Add(type, -GetBonus(-diff));
                Debug.Log(type + " - "+GetBonus(-diff));
            }
        }

        private int GetBonus(int diffAbs)
        {
            return diffAbs < Bonus ? Bonus - diffAbs : Bonus;
        }
    }
}