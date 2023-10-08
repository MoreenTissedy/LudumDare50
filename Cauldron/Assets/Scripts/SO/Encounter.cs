using System;
using System.Collections.Generic;
using EasyLoc;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New_Encounter", menuName = "Encounter", order = 1)]
    public class Encounter : LocalizableSO
    {
        [FormerlySerializedAs("addToDeckOnDay")] public int addToDeckOnRound = -1;
        public string requiredStoryTag;

        [Serializable]
        public class PotionResult
        {
            public Potions potion = Potions.DEFAULT;
            [Range(-1, 1)]
            public float influenceCoef = 1;
            public NightEvent bonusEvent;
            public Encounter bonusCard;
        }

        [FormerlySerializedAs("villager1")] public Villager villager;

        [TextArea(5, 10)]
        public string text;

        public bool hidden = false, quest = false;
        public Statustype primaryInfluence, secondaryInfluence = Statustype.None;
        public float primaryCoef, secondaryCoef;
        [ReorderableList]
        public PotionResult[] resultsByPotion = Array.Empty<PotionResult>();

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
    }
}