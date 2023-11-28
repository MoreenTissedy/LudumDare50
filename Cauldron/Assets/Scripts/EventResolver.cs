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
        }

        public void ApplyStoryTag(NightEvent nightEvent)
        {
            string storyTag = nightEvent.storyTag;
            if (storyTag.StartsWith("-"))
            {
                if (storyTag.StartsWith("*"))
                {
                    storyTag = storyTag.TrimStart('*');
                    StoryTagHelper.RemoveMilestone(storyTag);
                }
                game.storyTags.Remove(storyTag.TrimStart('-'));
            }
            else if (!string.IsNullOrEmpty(storyTag))
            {
                if (storyTag.StartsWith("*"))
                {
                    storyTag = storyTag.TrimStart('*');
                    StoryTagHelper.SaveMilestone(storyTag);
                }
                game.AddTag(storyTag);
                Debug.Log($"Add story tag: {storyTag}");
            }
        }

        public void ApplyFractionShift(FractionData shift)
        {
            game.fractionStatus.ChangeStatus(shift.Fraction, shift.Status);
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

            var random = GetRandomValidIndex(nightEvent.bonusCards);

            for (var i = 0; i < nightEvent.bonusCards.Length; i++)
            {
                if (i == random)
                {
                    continue;
                }
                if (!game.currentDeck.AddToDeck(nightEvent.bonusCards[i]))
                {
                    game.currentDeck.AddToPool(nightEvent.bonusCards[i]);
                }
            }
            return nightEvent.bonusCards[random];
        }

        private int GetRandomValidIndex(Encounter[] cards)
        {
            Encounter priority = null;
            int random = -1;
            for (int i = 0; i < 10; i++)
            {
                random = Random.Range(0, cards.Length);
                if (StoryTagHelper.Check(cards[random], game))
                {
                    priority = cards[random];
                    break;
                }
            }

            if (priority is null)
            {
                random = -1;
            }

            return random;
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