using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Skins Provider", menuName = "Skins Provider", order = 10)]
    public class SkinsProvider : ScriptableObject
    {
        public SkinSO initialSkin; 
        
        [ReorderableList] public SkinSO[] skins;

        private List<string> unlocked;
        private Dictionary<string, SkinSO> skinsDictionary;
        private IAchievementManager achievements;

        private readonly string fileName = "UnlockedSkins";
        private FileDataHandler<ListToSave<string>> fileDataHandler;
        
        //For legacy skin unlock
        private IReadOnlyList<string> unlockedEndings;

        public void Init(IAchievementManager achievementManager, EndingsProvider endingsProvider)
        {
            achievements = achievementManager;
            unlockedEndings = endingsProvider.UnlockedEndings;
            
            fileDataHandler = new FileDataHandler<ListToSave<string>>(fileName);
            skinsDictionary = new Dictionary<string, SkinSO>(12);
            unlocked = LoadUnlockedSkins();
            Save();
            
            foreach (var skin in skins)
            {
                skinsDictionary.Add(skin.name, skin);
            }
        }

        public int GetUnlockedSkinsCount()
        {
            return unlocked.Count;
        }

        public int GetMinimumPrice()
        {
            if (skins == null || skins.Length == 0)
            {
                throw new InvalidOperationException("The list is empty or null");
            }

            var positivePrices = skins.Where(skin => skin.Price > 0).Select(skin => skin.Price).ToList();

            if (!positivePrices.Any())
            {
                throw new InvalidOperationException("No positive prices found in the list");
            }

            return positivePrices.Min();
        }
        
        public bool Unlocked(string skin)
        {
            return unlocked.Contains(skin);
        }

        public bool Unlocked(SkinSO skin)
        {
            foreach (var keyValuePair in skinsDictionary)
            {
                if (keyValuePair.Value == skin)
                {
                    return Unlocked(keyValuePair.Value);
                }
            }
            return false;
        }

        public bool TryUnlock(SkinSO skin)
        {
            if (!Unlocked(skin.name))
            {
                unlocked.Add(skin.name);
                Debug.Log("Skin unlocked "+skin.name);
                Save();
                return true;
            }
            achievements.TryUnlock(skin.name);
            return false;
        }

        public void Reset()
        {
            unlocked.Clear();
            Save();
        }
        
        private List<string> LoadUnlockedSkins()
        {
            var list = fileDataHandler.IsFileValid()
                ? fileDataHandler.Load().list : new List<string> { "Default" };
            LoadLegacy(list);
            Debug.Log($"Skins unlocked {String.Join(", ", list)}");
            return list;
        }

        private void LoadLegacy(ICollection<string> list)
        {
            foreach (var skin in skins)
            {
                if(skin.LastUnlockedEnding.Any(ending => unlockedEndings.Contains(ending)))
                {
                    if (!list.Contains(skin.name))
                    {
                        list.Add(skin.name);
                    }
                }
            }
        }

        private void Save()
        {
            fileDataHandler.Save(new ListToSave<string>(unlocked));
        }

        [ContextMenu("Export Skins to CSV")]
        public void ExportEndings()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Skins.csv");
            file.WriteLine("id;string_RU;string_EN");
            
            foreach (var skinSo in skins)
            {
                file.WriteLine($"{skinSo.name}.title;{skinSo.FriendlyName}");
                file.WriteLine($"{skinSo.name}.description;{skinSo.FlavorText}");
                file.WriteLine($"{skinSo.name}.short;{skinSo.DescriptionText}");
            }
            file.Close();
        }
    }
}
