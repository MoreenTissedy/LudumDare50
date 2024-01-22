using System.Linq;
using UnityEngine;

namespace CauldronCodebase
{
    public class EncounterResolver
    {
        private readonly GameDataHandler game;
        private readonly EncounterDeck deck;
        private readonly NightEventProvider nightEvents;
        private readonly MainSettings settings;

        public EncounterResolver(MainSettings settings, GameDataHandler game, EncounterDeck deck, NightEventProvider nightEvents)
        {
            this.game = game;
            this.deck = deck;
            this.nightEvents = nightEvents;
            this.settings = settings;
        }
        
        public bool EndEncounter(Potions potion)
        {
            Encounter encounter = game.currentCard;
            
            settings.recipeHintsStorage.SaveHint(encounter.recipeHintConfig);
            
            //compare distinct potion
            foreach (var filter in encounter.resultsByPotion)
            {
                if (potion == filter.potion)
                {
                    ApplyResult(filter);
                    return potion != Potions.Placebo && filter.influenceCoef != 0;
                }
            }

            //evaluate potion filters
            if (PotionInFilter(Potions.ALCOHOL, out var alcFilter)) return alcFilter.influenceCoef != 0;
            if (PotionInFilter(Potions.DRINK, out var drinkFilter)) return drinkFilter.influenceCoef != 0;
            if (PotionInFilter(Potions.FOOD, out var foodFilter)) return foodFilter.influenceCoef != 0;
            if (PotionInFilter(Potions.MAGIC, out var magicFilter)) return magicFilter.influenceCoef != 0;
            if (PotionInFilter(Potions.NONMAGIC, out var nonMagicFilter)) return nonMagicFilter.influenceCoef != 0;
            var filterResult = encounter.resultsByPotion.FirstOrDefault(result => result.potion == Potions.DEFAULT);
            if (filterResult != null)
            {
                ApplyResult(filterResult);
                return filterResult.influenceCoef != 0;
            }
            return false;

            void ModifyStat(Statustype type, float statCoef, float potionCoef)
            {
                if (type == Statustype.None)
                {
                    return;
                }

                float defaultStatChange = type == Statustype.Money
                    ? settings.gameplay.defaultMoneyChangeCard
                    : settings.gameplay.defaultStatChange;
                
                game.Add(type,
                    Mathf.FloorToInt(defaultStatChange
                                     * statCoef
                                     * potionCoef));
            }

            void ApplyResult(Encounter.PotionResult potionResult)
            {
                ModifyStat(encounter.primaryInfluence, encounter.primaryCoef, potionResult.influenceCoef);
                ModifyStat(encounter.secondaryInfluence, encounter.secondaryCoef, potionResult.influenceCoef);
                if (potionResult.bonusCard != null)
                {
                    if (!deck.AddToDeck(potionResult.bonusCard))
                    {
                        game.currentDeck.AddToPool(potionResult.bonusCard);
                    }
                }
                if (potionResult.bonusEvent != null && StoryTagHelper.Check(potionResult.bonusEvent.requiredTag, game))
                {
                    nightEvents.storyEvents.Add(potionResult.bonusEvent);
                }
            }

            bool PotionInFilter(Potions filter, out Encounter.PotionResult data)
            {
                Encounter.PotionResult filterValue = encounter.resultsByPotion.FirstOrDefault(result => result.potion == filter);
                if (filterValue != null && PotionFilter.Get(filter).Contains(potion))
                {
                    ApplyResult(filterValue);
                    data = filterValue;
                    return true;
                }

                data = null;
                return false;
            }
        }
    }
}