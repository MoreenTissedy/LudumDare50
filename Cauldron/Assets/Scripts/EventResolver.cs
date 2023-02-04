using UnityEngine;

namespace CauldronCodebase
{
    public class EventResolver
    {
        private readonly GameData game;
        private readonly MainSettings settings;

        public EventResolver(MainSettings settings, GameData game)
        {
            this.game = game;
            this.settings = settings;
        }
        
        public void ApplyModifiers(NightEvent nightEvent)
        {
            game.Fame += CalculateModifier(Statustype.Fame, nightEvent);
            game.Fear += CalculateModifier(Statustype.Fear, nightEvent);
            game.Money += CalculateModifier(Statustype.Money, nightEvent);
            game.currentDeck.AddToDeck(nightEvent.bonusCard, true);
            string storyTag = nightEvent.storyTag;
            if (storyTag.StartsWith("-"))
            {
                game.storyTags.Remove(storyTag);
            }
            else if (!string.IsNullOrEmpty(storyTag))
            {
                game.AddTag(storyTag);
            }
        }

        public int CalculateModifier(Statustype type, NightEvent nightEvent)
        {
            switch (type)
            {
                case Statustype.Money:
                    return Mathf.FloorToInt(nightEvent.moneyCoef * settings.gameplay.defaultMoneyChangeEvent);
                case Statustype.Fear:
                    return Mathf.FloorToInt(nightEvent.fearCoef * settings.gameplay.defaultStatChange);
                case Statustype.Fame:
                    return Mathf.FloorToInt(nightEvent.fameCoef * settings.gameplay.defaultStatChange);
            }

            return 0;
        }
    }
}