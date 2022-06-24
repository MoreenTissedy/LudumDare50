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
        private static string assetFolder = "Cards";
        private static string CSVpath = "/Editor/Deck.csv";
        [MenuItem("Utilities/Import cards")]
        public static void Import()
        {
            if (!AssetDatabase.GetSubFolders("Assets").Contains($"Assets/{assetFolder}"))
            {
                AssetDatabase.CreateFolder("Assets", assetFolder);
            }
            string[] alllines = File.ReadAllLines(Application.dataPath + CSVpath);
            Villager[] allVillagers = ScriptableObjectHelper.LoadAllAssets<Villager>();

            foreach (var line in alllines)
            {
                string[] data = line.Split(';');
                
                //if SO exists in folder - take it
                bool newSO = false;
                Encounter card = AssetDatabase.LoadAssetAtPath<Encounter>($"Assets/{assetFolder}/{data[0]}.asset");
                if (card is null)
                {
                    card = ScriptableObject.CreateInstance<Encounter>();
                    newSO = true;
                }
                else
                {
                    newSO = false;
                    Debug.Log($"found existing {data[0]}, will overwrite");
                }
                //AddOnDay
                card.addToDeckOnDay = ConvertIntFromString(data[1]);

                //Villagers
                string[] villagersFromFile = data[2].Split(',');
                List<Villager> foundVillagers = new List<Villager>(2);
                foreach (var villager in allVillagers)
                {
                    foreach (var villagerData in villagersFromFile)
                    {
                        if (villager.name == villagerData.Trim())
                        {
                            foundVillagers.Add(villager);
                        }
                    }
                }
                card.villager = foundVillagers.ToArray();
                
                card.text = data[3];
               
                // 4. Starred — ставим +, если нужно пометить запрос для игрока звездочкой (обозначив сильное влияние на сюжет или получение предмета).
                // 5. Hidden — ставим +, если нужно скрыть для игрока влияние на статы Primary и Secondary, заменив их значком вопроса.
                // 6. Primary — стат, который меняется в результате запроса (Money, Fear, Fame, None).
                card.primaryInfluence = ParseStat(data[6]);
                // 7. Secondary — второй стат, который меняется в результате запроса (Money, Fear, Fame, None).
                card.secondaryInfluence = ParseStat(data[7]);
                // 8. PrimaryCoef — степень изменения первичного стата (коэффициент от дефолтного значения в системе). Любое число с точкой (0 для None).
                // 9. SecondaryCoef — то же для второго стата.
                // 10 и далее — коэффициенты изменения статов по отдельным зельям (пустое поле будет принято за 0), любое число с точкой. Порядок столбцов должен совпадать с порядком магических зелий в enum Potions
                RecipeProvider recipeProvider = ScriptableObjectHelper.LoadSingleAsset<RecipeProvider>();
                int column = 10;
                List<Encounter.PotionResult> newPotionResults = new List<Encounter.PotionResult>(3);
                foreach (Potions potion in Enum.GetValues(typeof(Potions)))
                {
                    if (!recipeProvider
                        .GetRecipeForPotion(potion)?
                        .magical ?? true)
                    {
                        continue;
                    }

                    int value = ConvertIntFromString(data[column]);
                    column++;
                    if (value == 0)
                        continue;

                    //look for existing resultByPotion
                    bool overridden = false;
                    foreach (Encounter.PotionResult potionResult in card.resultsByPotion)
                    {
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