using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New wrong potion event", menuName = "Night event/Wrong potions", order = 8)]
    public class WrongPotionCountEvent : ConditionalEvent
    {
        [Header("Wrong potion count threshold")]
        public int wrongPotions;

        public override bool Valid(GameData game)
        {
            return game.wrongPotionsCount >= wrongPotions;
        }
    }
}