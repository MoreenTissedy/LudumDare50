using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/ForgetPast")]
    public class ForgetPastGameMode : GameModeBase
    {
        private GameDataHandler gameDataHandler;
        public override bool ShouldReapply { get; } = false;

        [Inject]
        public void Construct(GameDataHandler gameDataHandler)
        {
            this.gameDataHandler = gameDataHandler;
        }

        public override void Apply()
        {
            gameDataHandler.storyTags.Clear();
            gameDataHandler.milestonesDisable = true;
        }
    }
}