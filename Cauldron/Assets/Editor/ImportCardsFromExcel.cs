using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using CauldronCodebase;

namespace Editor
{
    public class ImportCardsFromExcel
    {
        private static string CSVpath = "/Editor/Deck.csv";
        [MenuItem("Utilities/Import cards")]
        public static void Import()
        {
            AssetDatabase.CreateFolder("Assets", "Cards");
            
            string[] alllines = File.ReadAllLines(Application.dataPath + CSVpath);
            
            foreach (var line in alllines)
            {
                string[] data = line.Split(';');
                Encounter enc = ScriptableObject.CreateInstance<Encounter>();

                // foreach (var villager in allVillagers)
                // {
                //     Debug.Log(villager.name);
                //     if (villager.name == data[2])
                //     {
                //         enc.actualVillager = villager;
                //         enc.villager = new[] {villager};
                //         break;
                //     }
                // }
                
                enc.text = data[2];
                /*if (Enum.TryParse(data[3], out Potions potion))
                {
                    enc.requiredPotion = potion;
                }
                if (Enum.TryParse(data[4], out Potions potion2))
                {
                    enc.requiredPotion2 = potion2;
                }

                enc.moneyBonus = ConvertFromString(data[5]);
                enc.fearBonus = ConvertFromString(data[6]);
                enc.fameBonus = ConvertFromString(data[7]);
                //card - data8
                enc.moneyPenalty = ConvertFromString(data[9]);
                enc.fearPenalty = ConvertFromString(data[10]);
                enc.famePenalty = ConvertFromString(data[11]);*/
                //card penalty

                AssetDatabase.CreateAsset(enc, $"Assets/Cards/{data[0]}.asset");
            }
            AssetDatabase.SaveAssets();
        }

        static int ConvertFromString(string data)
        {
            if (Int32.TryParse(data, out int number))
            {
                return number;
            }
            else
            {
                return 0;
            }
        }
    }
}