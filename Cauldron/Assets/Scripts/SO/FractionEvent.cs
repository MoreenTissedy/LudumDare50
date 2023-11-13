using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Fraction event", menuName = "Night event/Fraction", order = 8)]
    public class FractionEvent: ConditionalEvent
    {
        public FractionData threshold;
        private GameDataHandler cachedGameData;
        public override bool Valid(GameDataHandler game)
        {
            cachedGameData = game;
            return game.fractionStatus.GetStatus(threshold.Fraction) >= threshold.Status && !game.fractionEventTriggered;
        }

        public override void OnResolve()
        {
            base.OnResolve();
            if (cachedGameData)
            {
                cachedGameData.fractionEventTriggered = true;
            }
        }
    }
}