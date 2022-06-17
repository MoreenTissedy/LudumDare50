using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using CauldronCodebase;

namespace Editor
{
    public class ContentImportExport
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

        [MenuItem("Utilities/Export for GraphViz")]
        public static void ExportToDot()
        {
            //skip this for now
            RandomNightEvent[] startingEvents = ScriptableObjectHelper.LoadAllAssets<RandomNightEvent>();
            Encounter[] encounters = ScriptableObjectHelper.LoadAllAssets<Encounter>();

            List<NightEvent> storyEvents = new List<NightEvent>(30);
            string dotNotation = "digraph{\n";
            foreach (var encounter in encounters)
            {
                if (encounter.addToDeckOnDay >= 0)
                {
                    dotNotation += $"{encounter.name} [style=filled, color=yellow]\n";
                }
                else
                {
                    dotNotation += $"{encounter.name}\n";
                }
                
                bool endNode = true;
                foreach (Encounter.PotionResult result in encounter.resultsByPotion)
                {
                    string color = result.influenceCoef > 0 ? "green" : "red";
                    string style = "dotted";
                    if (Mathf.Abs(result.influenceCoef) >= 0.8f) style = "solid";
                    else if (Mathf.Abs(result.influenceCoef) >= 0.5f) style = "dashed";
                    
                    if (result.bonusCard != null)
                    {
                        endNode = false;
                        dotNotation +=
                            $"{encounter.name} -> {result.bonusCard.name} [color={color}, label={result.potion}, style={style}]\n";
                    }
                    if (result.bonusEvent != null)
                    {
                        endNode = false;
                        storyEvents.Add(result.bonusEvent);
                        dotNotation += $"{encounter.name} -> {result.bonusEvent.name}[color={color}, label={result.potion}, style = {style}]\n";
                        dotNotation += $"{result.bonusEvent.name} [shape = record]\n";
                    }
                }
                if (endNode == true)
                {
                    //dotNotation += $"{encounter.name} [color=purple]\n";
                }
            }
            foreach (NightEvent storyEvent in storyEvents)
            {
                if (storyEvent.bonusCard != null)
                {
                    dotNotation += $"{storyEvent.name} -> {storyEvent.bonusCard.name}\n";
                }
            }
            dotNotation += "}";
            File.WriteAllText(Application.dataPath + "/Content.dot", dotNotation);
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