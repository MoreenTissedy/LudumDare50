using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Skins Provider", menuName = "Skins Provider", order = 10)]
    public class SkinsProvider : ScriptableObject
    {
        public SkinSO[] skins;

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

        public void TryUnlock(SkinSO skin)
        {
            if (!Unlocked(skin.name))
            {
                unlocked.Add(skin.name);
                Save();
            }
            achievements.TryUnlock(skin.name);
        }

        public void Reset()
        {
            unlocked.Clear();
            Save();
        }
        
        private List<string> LoadUnlockedSkins()
        {
            var list = fileDataHandler.IsFileValid()
                ? fileDataHandler.Load().list : new List<string> { "main" };
            LoadLegacy(list);
            return list;
        }

        private void LoadLegacy(ICollection<string> list)
        {
            foreach (var skin in skins)
            {
                if(skin.LastUnlockedEnding.Any(ending => unlockedEndings.Contains(ending)));
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
    }
}
