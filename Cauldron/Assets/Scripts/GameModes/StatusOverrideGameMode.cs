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

        [Inject]
        public void Construct(GameDataHandler gameDataHandler)
        {
            this.gameDataHandler = gameDataHandler;
        }
        public override void Apply()
        {
            gameDataHandler.StatusChanged += CheckStatus;
        }

        private void CheckStatus(Statustype type, int diff)
        {
            if (type != Status || diff == 0)
            {
                return;
            }

            if (diff > 0 && Up)
            {
                gameDataHandler.Add(type, GetBonus(diff));
                Debug.LogError("+ "+GetBonus(diff));
            }

            if (diff < 0 && Down)
            {
                gameDataHandler.Add(type, -GetBonus(-diff));
                Debug.LogError("- "+GetBonus(-diff));
            }
        }

        private int GetBonus(int diffAbs)
        {
            return diffAbs < Bonus ? Bonus - diffAbs : Bonus;
        }
    }
}