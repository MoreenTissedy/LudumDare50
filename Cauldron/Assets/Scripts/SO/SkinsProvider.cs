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
        [ReorderableList] public SkinSO[] skins;

        private List<string> unlocked;
        private Dictionary<string, SkinSO> skinsDictionary;
        private IAchievementManager achievements;

        private readonly string fileName = "UnlockedSkins";
        private FileDataHandler<ListToSave<string>> fileDataHandler;
        
        //For legacy skin unlock
        private EndingsProvider endings;

        public SkinSO Get(string name)
        {
            return skins.First(x => x.name == name);
        }

        public void Init(IAchievementManager achievementManager, EndingsProvider endingsProvider)
        {
            achievements = achievementManager;
            endings = endingsProvider;
            
            fileDataHandler = new FileDataHandler<ListToSave<string>>(fileName);
            skinsDictionary = new Dictionary<string, SkinSO>(12);
            unlocked = LoadUnlockedSkins();
            Save();
            
            foreach (var skin in skins)
            {
                OverridePriceForFrogSkin(skin);
                skinsDictionary.Add(skin.name, skin);
            }

            void OverridePriceForFrogSkin(SkinSO skin)
            {
                if ((skin.name == "Frog") && endingsProvider.Unlocked(EndingsProvider.ENOUGH_MONEY))
                {
                    skin.Price = 100;
                }
            }
        }

        public int GetUnlockedSkinsCount()
        {
            return unlocked.Count;
        }

        public SkinSO GetInitialSkin()
        {
            if (PlayerPrefs.HasKey(PrefKeys.LastPlayerSkin))
                return Get(PlayerPrefs.GetString(PrefKeys.LastPlayerSkin)); 
            for (var i = unlocked.Count - 1; i >= 0; i--)
            {
                SkinSO skin = Get(unlocked[i]);
                if (!skin.NeedsApprove)
                {
                    return skin;
                }
            }
            return Get(unlocked[0]);
        }

        public int GetMinimumPrice()
        {
            if (skins == null || skins.Length == 0)
            {
                throw new InvalidOperationException("The list is empty or null");
            }

            var positivePrices = new List<int>();
            foreach (var skin in skins)
            {
                if (!unlocked.Contains(skin.name) && skin.Price > 0) positivePrices.Add(skin.Price);
            }

            if (!positivePrices.Any())
            {
                return int.MaxValue;
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
                achievements.TryUnlock("skin "+skin.name.ToLowerInvariant());
                return true;
            }
            return false;
        }
        
        private List<string> LoadUnlockedSkins()
        {
            var list = fileDataHandler.IsFileValid()
                ? fileDataHandler.Load().list : new List<string> { "Default" };
            TryLoadLegacy(list);
            Debug.Log($"Skins unlocked {String.Join(", ", list)}");
            return list;
        }

        private void TryLoadLegacy(ICollection<string> list)
        {
            if (!endings.Legacy)
            {
                return;
            }
            foreach (var skin in skins)
            {
                if (skin.LastUnlockedEnding.Any(ending => endings.UnlockedEndings.Contains(ending)))
                {
                    if (!list.Contains(skin.name))
                    {
                        list.Add(skin.name);
                        achievements.TryUnlock("skin "+skin.name.ToLowerInvariant());
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
