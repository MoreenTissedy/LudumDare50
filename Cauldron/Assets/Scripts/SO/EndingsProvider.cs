using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Endings Provider", menuName = "Endings Provider", order = 10)]
    public class EndingsProvider : ScriptableObject
    {
        public const string HIGH_FAME = "high fame";
        public const string LOW_FAME = "low fame";
        public const string HIGH_FEAR = "high fear";
        public const string LOW_FEAR = "low fear";
        public const string ENOUGH_MONEY = "circle";
        public const string FINAL = "Bobby";
        public const string END_DECK = "moving";
        public const string KING_GOOD = "king success";
        public const string KING_BAD = "king failure";
        public const string BISHOP_GOOD = "bishop success";
        public const string BISHOP_BAD = "bishop failure";
        public const string BANDIT = "bandit revolt";
        
        public Ending[] endings;

        private List<string> unlocked;
        private Dictionary<string, Ending> endingDict;
        private IAchievementManager achievements;

        public void Init(IAchievementManager achievements)
        {
            this.achievements = achievements;
            
            endingDict = new Dictionary<string, Ending>(12);
            foreach (var ending in endings)
            {
                endingDict.Add(ending.tag, ending);
            }

            if (PlayerPrefs.HasKey(PrefKeys.UnlockedEndings))
            {
                unlocked = PlayerPrefs.GetString(PrefKeys.UnlockedEndings).Split(',').ToList();
            }
            else
            {
                unlocked = new List<string>();
            }
        }

        public int GetUnlockedEndingsCount()
        {
            return unlocked.Count;
        }

        public bool Unlocked(string tag)
        {
            return unlocked.Contains(tag);
        }

        public bool Unlocked(Ending ending)
        {
            foreach (var keyValuePair in endingDict)
            {
                if (keyValuePair.Value == ending)
                {
                    return Unlocked(keyValuePair.Key);
                }
            }
            return false;
        }
        
        public Ending Get(string tag)
        {
            return endingDict[tag];
        }

        public void TryUnlock(string tag)
        {
            if (tag != "none" && !Unlocked(tag))
            {
                unlocked.Add(tag);
                PlayerPrefs.SetString(PrefKeys.UnlockedEndings, string.Join(",", unlocked));
            }
            achievements.TryUnlock(tag);
        }

        [ContextMenu("Export Endings to CSV")]
        public void ExportEndings()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Endings.csv");
            file.WriteLine("id;string_RU;string_EN");
            foreach (var ending in endings)
            {
                file.WriteLine($"{ending.name}.title;{ending.title}");
                file.WriteLine($"{ending.name}.description;{ending.text}");
                file.WriteLine($"{ending.name}.short;{ending.shortTextForEndingAnimation}");
            }
            file.Close();
        }
    }
}