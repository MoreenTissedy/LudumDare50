using System;
using System.Collections.Generic;
using EasyLoc;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New_Encounter", menuName = "Encounter", order = 1)]
    public class Encounter : LocalizableSO
    {
        public Villager[] villager;
        [TextArea(5, 10)]
        public string text;
        public Potions requiredPotion;
        [Header("Right potion brewed")] public int moneyBonus; 
        public int fearBonus, fameBonus;
        public Encounter[] bonusCard;
        [Header("Wrong potion brewed")] public int moneyPenalty;
        public int fearPenalty, famePenalty;
        public Encounter[] penaltyCard;
        [Header("Second potion variant")] 
        public bool useSecondVariant;
        public Potions requiredPotion2;
        public int moneyBonus2; 
        public int fearBonus2, fameBonus2;
        public Encounter[] bonusCard2;

        [HideInInspector] public Villager actualVillager;

        public void Init()
        {
            if (villager.Length > 0)
            {
                int random = Random.Range(0, villager.Length);
                actualVillager = villager[random];
            }
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
                    text = data[requiredColumns[0]];
                    return true;
                }
            }

            return false;
        }
    }
}