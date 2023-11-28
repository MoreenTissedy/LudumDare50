using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "CircleEndingEvent", menuName = "Night event/Money special event", order = 10)]
    public class MoneyCircleEvent: ConditionalEvent
    {
        public override bool Valid(GameDataHandler game)
        {
            return game.Money >= game.statusSettings.MoneyTotal;
        }
    }
}