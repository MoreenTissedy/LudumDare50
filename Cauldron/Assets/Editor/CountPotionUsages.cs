using System;
using System.Collections.Generic;
using UnityEngine;
using Editor;
using UnityEditor;

namespace CauldronCodebase
{
    public class CountPotionUsages
    {
        [MenuItem("Utilities/Count Potion Usage")]
        public static void Count()
        {
            Debug.Log("Calculating potion usages in all encounters...");
            Dictionary<Potions, int> dictionary = new Dictionary<Potions, int>(Enum.GetValues(typeof(Potions)).Length);
            Encounter[] encounters = ScriptableObjectHelper.LoadAllAssets<Encounter>();
            foreach (var encounter in encounters)
            {
                foreach (var potionResult in encounter.resultsByPotion)
                {
                    if (dictionary.TryGetValue(potionResult.potion, out int count))
                    {
                        dictionary[potionResult.potion]++;
                    }
                    else
                    {
                        dictionary.Add(potionResult.potion, 1);
                    }
                }
            }
            foreach (var keyValuePair in dictionary)
            {
                Debug.Log(keyValuePair.Key+": "+keyValuePair.Value);
            }
        }

    }
}