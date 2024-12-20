using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using CauldronCodebase;

namespace Editor
{
    public class ContentImportExport
    {
        private static string assetFolder = "[Syomalod] Cards";
        private static string CSVpath = "/Editor/SingleCards.csv";

        private static string eventFolder = "[Syomalod] Events";
        private static string CSVpath4events = "/Editor/SkinEvents2.csv";

        [MenuItem("Utilities/Find event by tag")]
        public static void FindEventByTag()
        {
            const string tag = "bishop success";
            foreach (var nightEvent in ScriptableObjectHelper.LoadAllAssets<NightEvent>())
            {
                if (nightEvent.storyTag.Split(',').Any(x => x.Contains(tag)))
                {
                    Debug.LogError(nightEvent.name);
                }
            }
        }

        [MenuItem("Utilities/Import events")]
        public static void ImportEvents()
        {
            if (!AssetDatabase.GetSubFolders("Assets").Contains($"Assets/{eventFolder}"))
            {
                AssetDatabase.CreateFolder("Assets", eventFolder);
            }
            string[] alllines = File.ReadAllLines(Application.dataPath + CSVpath4events);
            for (var index = 1; index < alllines.Length; index++)
            {
                var line = alllines[index];
                string[] data = line.Split(';');

                bool newSO = false;
                NightEvent card;

                //find or create night event of corresponding type
                string[] randomData = data[1].Split(':');
                if (ConvertIntFromString(randomData[0]) == -1)
                {
                    card = AssetDatabase.LoadAssetAtPath<NightEvent>($"Assets/{eventFolder}/{data[0]}.asset");
                    if (card != null)
                    {
                        newSO = false;
                        Debug.Log($"found existing {data[0]}, will overwrite");
                    }
                    else
                    {
                        card = ScriptableObject.CreateInstance<NightEvent>();
                        card.name = data[0];
                        Debug.Log($"reading {data[0]}...");
                        newSO = true;
                    }
                }
                else
                {
                    card = AssetDatabase.LoadAssetAtPath<RandomNightEvent>($"Assets/{eventFolder}/{data[0]}.asset");
                    if (card != null)
                    {
                        newSO = false;
                        Debug.Log($"found existing {data[0]}, will overwrite");
                    }
                    else
                    {
                        card = ScriptableObject.CreateInstance<RandomNightEvent>();
                        card.name = data[0];
                        Debug.Log($"reading {data[0]}...");
                        newSO = true;
                    }
                    RandomNightEvent randomCard = (RandomNightEvent) card;
                    randomCard.minDay = ConvertIntFromString(randomData[0]);
                }
                //Title & Text
                card.title = data[1];
                card.flavourText = data[2];
                //Coefs
                card.moneyCoef = ConvertFloatFromString(data[3]);
                card.fearCoef = ConvertFloatFromString(data[4]);
                card.fameCoef = ConvertFloatFromString(data[5]);
                if (newSO)
                {
                    AssetDatabase.CreateAsset(card, $"Assets/{eventFolder}/{card.name}.asset");
                }

                AssetDatabase.SaveAssets();
            }
        }
        
        [MenuItem("Utilities/Import cards")]
        public static void Import()
        {
            if (!AssetDatabase.GetSubFolders("Assets").Contains($"Assets/{assetFolder}"))
            {
                AssetDatabase.CreateFolder("Assets", assetFolder);
            }
            string[] alllines = File.ReadAllLines(Application.dataPath + CSVpath);
            Villager[] allVillagers = ScriptableObjectHelper.LoadAllAssets<Villager>();

            string[] headers = alllines[0].Split(';');
            List<Potions> includedPotionResults = new List<Potions>(10);
            int i = 10;
            while (i < headers.Length)
            {
                if (Enum.TryParse(headers[i], true, out Potions potionParsed))
                {
                    includedPotionResults.Add(potionParsed);
                    i++;
                }
                else
                {
                    Debug.LogWarning("Parsing potions stopped at "+headers[i]);
                    break;
                }
            }
            
            for (var index = 1; index < alllines.Length; index++)
            {
                var line = alllines[index];
                string[] data = line.Split(';');

                //if SO exists in folder - take it
                bool newSO = false;
                Encounter card = AssetDatabase.LoadAssetAtPath<Encounter>($"Assets/{assetFolder}/{data[0]}.asset");
                if (card is null)
                {
                    card = ScriptableObject.CreateInstance<Encounter>();
                    card.name = data[0];
                    newSO = true;
                }
                else
                {
                    newSO = false;
                    Debug.Log($"found existing {data[0]}, will overwrite");
                }

                //AddOnDay
                card.addToDeckOnRound = ConvertIntFromString(data[1]);

                //Villager
                bool foundVillager = false;
                foreach (var villager in allVillagers)
                {
                    if (villager.name == data[2].Trim())
                    {
                        card.villager = villager;
                        foundVillager = true;
                        break;
                    }
                }
                if (!foundVillager && !string.IsNullOrWhiteSpace(data[2]))
                {
                    Debug.LogWarning("Failed to parse "+data[2]);
                }

                card.text = data[3];

                // 4. Starred — ставим +, если нужно пометить запрос для игрока звездочкой (обозначив сильное влияние на сюжет или получение предмета).
                //card.quest = data[4].Contains('+');
                // 5. Hidden — ставим +, если нужно скрыть для игрока влияние на статы Primary и Secondary, заменив их значком вопроса.
                card.hidden = data[5].Contains('+');
                // 6. Primary — стат, который меняется в результате запроса (Money, Fear, Fame, None).
                card.primaryInfluence = ParseStat(data[6]);
                // 7. Secondary — второй стат, который меняется в результате запроса (Money, Fear, Fame, None).
                card.secondaryInfluence = ParseStat(data[7]);
                // 8. PrimaryCoef — степень изменения первичного стата (коэффициент от дефолтного значения в системе). Любое число с точкой (0 для None).
                card.primaryCoef = ConvertFloatFromString(data[8]);
                // 9. SecondaryCoef — то же для второго стата.
                card.secondaryCoef = ConvertFloatFromString(data[9]);
                // 10 и далее — коэффициенты изменения статов по отдельным зельям (пустое поле будет принято за 0), любое число с точкой. Порядок столбцов должен совпадать с порядком магических зелий в enum Potions
                int column = 10;
                List<Encounter.PotionResult> newPotionResults = new List<Encounter.PotionResult>(3);
                foreach (Potions potion in includedPotionResults)
                {
                    float value = ConvertFloatFromString(data[column]);
                    column++;
                    if (value == 0)
                        continue;

                    //look for existing resultByPotion
                    bool overridden = false;
                    foreach (Encounter.PotionResult potionResult in card.resultsByPotion)
                    {
                        if (potionResult is null)
                        {
                            continue;
                        }

                        if (potionResult.potion == potion)
                        {
                            potionResult.influenceCoef = value;
                            overridden = true;
                            break;
                        }
                    }

                    if (!overridden)
                    {
                        //create new potion result 
                        Encounter.PotionResult newResult = new Encounter.PotionResult();
                        newResult.potion = potion;
                        newResult.influenceCoef = value;
                        newPotionResults.Add(newResult);
                    }
                }

                if (newPotionResults.Count > 0)
                {
                    card.resultsByPotion = card.resultsByPotion.Concat(newPotionResults).ToArray();
                }

                if (newSO)
                {
                    AssetDatabase.CreateAsset(card, $"Assets/{assetFolder}/{card.name}.asset");
                }
            }

            AssetDatabase.SaveAssets();
        }

        
        [MenuItem("Utilities/Export for GraphViz")]
        public static void ExportToDot()
        {
            RandomNightEvent[] startingEvents = ScriptableObjectHelper.LoadAllAssets<RandomNightEvent>();
            Encounter[] encounters = ScriptableObjectHelper.LoadAllAssets<Encounter>();

            List<NightEvent> storyEvents = new List<NightEvent>(30);
            string dotNotation = "digraph{\n";
            
            foreach (var randomNightEvent in startingEvents)
            {
                string name = randomNightEvent.name.Replace(".", "_").Replace("-", "_");
                dotNotation += $"{name} [shape = record]\n";
            }
            
            foreach (var encounter in encounters)
            {
                string encounterName = encounter.name.Replace(".", "_").Replace("-", "_");
                if (encounter.addToDeckOnRound >= 0)
                {
                    dotNotation += $"{encounterName} [style=filled, color=yellow]\n";
                }
                else
                {
                    dotNotation += $"{encounterName}\n";
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
                        string bonusCard = result.bonusCard.name.Replace(".", "_").Replace("-", "_");
                        endNode = false;
                        dotNotation +=
                            $"{encounterName} -> {bonusCard} [color={color}, label={result.potion}, style={style}]\n";
                    }
                    if (result.bonusEvent != null)
                    {
                        string bonusEvent = result.bonusEvent.name.Replace(".", "_").Replace("-", "_");
                        endNode = false;
                        storyEvents.Add(result.bonusEvent);
                        dotNotation += $"{encounterName} -> {bonusEvent} [color={color}, label={result.potion}, style = {style}]\n";
                        dotNotation += $"{bonusEvent} [shape = record]\n";
                    }
                }
                if (endNode == true)
                {
                    //dotNotation += $"{encounter.name} [color=purple]\n";
                }
            }
            foreach (NightEvent storyEvent in storyEvents.Union(startingEvents))
            {
                string eventName = storyEvent.name.Replace(".", "_").Replace("-", "_");
                foreach (var card in storyEvent.bonusCards)
                {
                    if (card is null)
                    {
                        continue;
                    }
                    string bonusCard = card.name.Replace(".", "_").Replace("-", "_");
                    dotNotation += $"{eventName} -> {bonusCard}\n";
                }
            }
            dotNotation += "}";
            File.WriteAllText(Application.dataPath + "/Content.dot", dotNotation);
        }

        static Statustype ParseStat(string data)
        {
            if (Enum.TryParse(data, true, out Statustype stat))
            {
                return stat;
            }
            else
            {
                return Statustype.None;
            }
        }
        static int ConvertIntFromString(string data)
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

        static float ConvertFloatFromString(string data)
        {
            if (float.TryParse(data, out float number))
            {
                return number;
            }
            else
            {
                return 0f;
            }
        }
    }
}