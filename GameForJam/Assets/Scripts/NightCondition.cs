using UnityEngine;
using EasyLoc;
using System.Collections.Generic;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New condition", menuName = "Total potion check condition", order = 8)]
    public class NightCondition : LocalizableSO
    {
        [Header("Conditions work only once")]
        public Potions type;
        public int threshold = 3;
        [TextArea(3, 10)]
        public string flavourText;
        public int moneyModifier, fearModifier, fameModifier;
        public Encounter bonusCard;
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
                if (headers[i].Contains("_"+language.ToString()))
                {
                    requiredColumns.Add(i);
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (data[0] == name)
                {
                    flavourText = data[requiredColumns[0]];
                    return true;
                }
            }
            return false;
        }
    }
}