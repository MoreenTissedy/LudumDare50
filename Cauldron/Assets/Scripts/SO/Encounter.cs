using System;
using System.Collections.Generic;
using EasyLoc;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New_Encounter", menuName = "Encounter", order = 1)]
    public class Encounter : LocalizableSO
    {
        private GameManager gm;

        public int addToDeckOnDay = -1;

        [Serializable]
        //TODO custom property drawer
        public class PotionResult
        {
            public Potions potion;
            [Range(-1, 1)]
            public float influenceCoef = 1;
            public NightEvent bonusEvent;
            public Encounter bonusCard;
        }

        public Villager[] villager;

        [TextArea(5, 10)]
        public string text;

        public bool hidden = false, quest = false;
        public Statustype primaryInfluence, secondaryInfluence = Statustype.None;
        public float primaryCoef, secondaryCoef;
        public PotionResult[] resultsByPotion = new PotionResult[3];

        [HideInInspector] public Villager actualVillager;


        
        public void Init(GameManager gameManager)
        {
            if (villager.Length > 0)
            {
                int random = Random.Range(0, villager.Length);
                actualVillager = villager[random];
            }

            gm = gameManager;
        }

        public static Encounter GetRandom(Encounter[] set)
        {
            if (set.Length > 0)
            {
                int random = Random.Range(0, set.Length);
                return (set[random]);
            }

            return null;
        }

        public override bool Localize(Language language)
        {
            if (localizationCSV == null)
                return false;
            //cache??
            string[] lines = localizationCSV.text.Split('\n');
            List<int> requiredColumns = new List<int>();
            string[] headers = lines[0].Split(';');
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+language))
                {
                    requiredColumns.Add(i);
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (data[0] == name)
                {
                    text = data[requiredColumns[0]];
                    return true;
                }
            }

            return false;
        }

        public bool EndEncounter(Potions potion, MainSettings settings)
        {
            //compare potion
            foreach (var filter in resultsByPotion)
            {
                if (potion == filter.potion)
                {
                    ModifyStat(primaryInfluence, primaryCoef, filter.influenceCoef);
                    ModifyStat(secondaryInfluence, secondaryCoef, filter.influenceCoef);
                    if (filter.bonusCard!=null)
                        gm.CardDeck.AddCardToPool(filter.bonusCard);
                    if (filter.bonusEvent!=null)
                        gm.NightEvents.storyEvents.Add(filter.bonusEvent);
                    if (filter.influenceCoef > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
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
                
                gm.GameState.Add(type,
                    Mathf.FloorToInt(defaultStatChange
                                     * statCoef
                                     * potionCoef));
            }
        }
    }
}