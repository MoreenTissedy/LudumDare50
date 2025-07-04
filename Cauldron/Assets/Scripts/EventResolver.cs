using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class EventResolver
    {
        private readonly GameDataHandler game;
        private readonly MainSettings settings;
        private readonly EncounterDeck deck;
        private readonly MilestoneProvider milestoneProvider;

        public EventResolver(MainSettings settings, GameDataHandler game, EncounterDeck deck, MilestoneProvider milestoneProvider)
        {
            this.game = game;
            this.deck = deck;
            this.settings = settings;
            this.milestoneProvider = milestoneProvider;
        }
        
        public void ApplyModifiers(NightEvent nightEvent)
        {
            game.Fame += CalculateModifier(Statustype.Fame, nightEvent);
            game.Fear += CalculateModifier(Statustype.Fear, nightEvent);
            game.Money += CalculateModifier(Statustype.Money, nightEvent);
        }

        public void ApplyStoryTag(NightEvent nightEvent)
        {
            string[] storyTags = nightEvent.storyTag.Split(',').Select(x => x.Trim()).ToArray();
            foreach (string tag in storyTags)
            {
                string storyTag = tag;
                storyTag = storyTag.TrimStart('^');
                if (storyTag.StartsWith("-", StringComparison.Ordinal))
                {
                    storyTag = storyTag.TrimStart('-');
                    
                    if (storyTag.StartsWith("*", StringComparison.Ordinal) && !game.milestonesDisable)
                    {
                        storyTag = storyTag.TrimStart('*');
                        milestoneProvider.RemoveMilestone(storyTag);
                    }
                    game.storyTags.Remove(storyTag);
                }
                else if (!string.IsNullOrEmpty(storyTag))
                {
                    if (storyTag.StartsWith("*", StringComparison.Ordinal) && !game.milestonesDisable)
                    {
                        storyTag = storyTag.TrimStart('*');
                        milestoneProvider.SaveMilestone(storyTag);
                    }
                    else if (storyTag.StartsWith("%", StringComparison.Ordinal) && !game.milestonesDisable)
                    {
                        Freezes.SaveFreeze(storyTag.Trim('%'));
                        continue;
                    }
                    game.AddTag(storyTag);
                }
            }
        }

        public void ApplyRecipeHint(RecipeHintConfig config)
        {
            settings.recipeHintsStorage.SaveHint(config);
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

            Encounter priority = null;
            for (var i = 0; i < nightEvent.bonusCards.Length; i++)
            {
                var nightEventBonusCard = nightEvent.bonusCards[i];
                if (priority is null && !deck.IsCardNotValidForDeck(nightEventBonusCard))
                {
                    priority = nightEventBonusCard;
                    continue;
                }
                if (game.currentDeck.AddToDeck(nightEventBonusCard))
                {
                    continue;
                }
                if (!EncounterIdents.GetAllSpecialCharacters().Contains(nightEventBonusCard.villager.name))
                {
                    game.currentDeck.AddToPool(nightEventBonusCard);
                }
            }
            return priority;
        }

        private int GetRandomValidIndex(Encounter[] cards)
        {
            Encounter priority = null;
            int random = -1;
            for (int i = 0; i < 10; i++)
            {
                random = Random.Range(0, cards.Length);
                if (!deck.IsCardNotValidForDeck(cards[random]))
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