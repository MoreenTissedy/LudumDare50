using UnityEngine;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class EventResolver
    {
        private readonly GameDataHandler game;
        private readonly MainSettings settings;

        public EventResolver(MainSettings settings, GameDataHandler game)
        {
            this.game = game;
            this.settings = settings;
        }
        
        public void ApplyModifiers(NightEvent nightEvent)
        {
            game.Fame += CalculateModifier(Statustype.Fame, nightEvent);
            game.Fear += CalculateModifier(Statustype.Fear, nightEvent);
            game.Money += CalculateModifier(Statustype.Money, nightEvent);
            string storyTag = nightEvent.storyTag;
            if (storyTag.StartsWith("-"))
            {
                game.storyTags.Remove(storyTag);
            }
            else if (!string.IsNullOrEmpty(storyTag))
            {
                game.AddTag(storyTag);
                Debug.Log($"Add story tag: {storyTag}");
            }
        }

        public Encounter AddBonusCards(NightEvent nightEvent)
        {
            if (nightEvent.bonusCards is null || nightEvent.bonusCards.Length == 0)
            {
                return null;
            }

            if (nightEvent.bonusCards.Length == 1)
            {
                return nightEvent.bonusCards[0];
            }
            
            int random = Random.Range(0, nightEvent.bonusCards.Length);
            for (var i = 0; i < nightEvent.bonusCards.Length; i++)
            {
                if (i == random)
                {
                    continue;
                }
                if (!game.currentDeck.AddToDeck(nightEvent.bonusCard))
                {
                    game.currentDeck.AddToPool(nightEvent.bonusCard);
                }
            }
            return nightEvent.bonusCards[random];
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