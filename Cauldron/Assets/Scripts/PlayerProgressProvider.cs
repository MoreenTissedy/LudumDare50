using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CauldronCodebase
{
    [Serializable]
    public class PlayerProgress
    {
        public List<int> UnlockedRecipes = new List<int>();
        public List<string> UnlockedEndings = new List<string>();
        public List<string> Milestones = new List<string>();
        public int CurrentRound = 0;
        public bool CovenIntroShown;
        public bool IsAutoCookingUnlocked;
    }

    public class PlayerProgressProvider
    {
        private readonly string fileName = "PlayerProgress";
        private FileDataHandler<PlayerProgress> fileDataHandler;
        
        public PlayerProgress progress;

        public List<int> UnlockedRecipes => progress.UnlockedRecipes;
        public List<string> UnlockedEndings => progress.UnlockedEndings;
        public List<string> Milestones => progress.Milestones;
        public int CurrentRound => progress.CurrentRound;    
        public bool CovenIntroShown => progress.CovenIntroShown;
        public bool IsAutoCookingUnlocked => progress.IsAutoCookingUnlocked;
        
        public PlayerProgressProvider()
        {
            fileDataHandler  = new FileDataHandler<PlayerProgress>(fileName);
            progress = GetPlayerProgress();
        }

        public void SaveCurrentRound(int round)
        {
            progress.CurrentRound = round;
            Debug.Log($"round saved");

            SaveProgress();
        }

        public void SaveCovenIntroShown()
        {
            progress.CovenIntroShown = true;
            Debug.Log($"CovenIntroShown saved");

            SaveProgress();
        }

        public void SaveAutoCookingUnlocked()
        {
            progress.IsAutoCookingUnlocked = true;
            Debug.Log($"IsAutoCookingUnlocked saved");

            SaveProgress();
        }
        
        public void SaveProgress()
        {
            fileDataHandler.Save(progress);
        }

        private PlayerProgress GetPlayerProgress()
        {
            if (TryLoadLegacy(out var legacyProgress))
            {
                return legacyProgress;
            }
            return fileDataHandler.IsFileValid() ? fileDataHandler.Load() : new PlayerProgress();
        }

        private bool TryLoadLegacy(out PlayerProgress legacyProgress)
        {
            legacyProgress = new PlayerProgress();
            bool hasLegacy = false;

            hasLegacy |= GetLegacyRecipes(ref legacyProgress.UnlockedRecipes);
            hasLegacy |= GetLegacyEndings(ref legacyProgress.UnlockedEndings);        
            hasLegacy |= GetLegacyMilestones(ref legacyProgress.Milestones);        
            hasLegacy |= GetLegacyRound(ref legacyProgress.CurrentRound);        
            hasLegacy |= GetLegacyCovenIntroShown(ref legacyProgress.CovenIntroShown);
            hasLegacy |= GetLegacyAutoCooking(ref legacyProgress.IsAutoCookingUnlocked);

            if (hasLegacy)
            {
                fileDataHandler.Save(legacyProgress);
                return true;
            }        
            legacyProgress = null;
            return false;    
        }

        private bool GetLegacyRecipes(ref List<int> list)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.UnlockedRecipes))
            {
                list = null;
                return false;
            }
            
            list = new List<int>();
            string data = PlayerPrefs.GetString(PrefKeys.UnlockedRecipes);
            foreach (var potion in data.Split(','))
            {
                if (string.IsNullOrWhiteSpace(potion))
                {
                    continue;
                }
                list.Add(int.Parse(potion));
            }
            PlayerPrefs.DeleteKey(PrefKeys.UnlockedRecipes);

            return true;
        }

        private bool GetLegacyEndings(ref List<string> list)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.UnlockedEndings))
            {
                list = null;
                return false;
            }

            list = PlayerPrefs.GetString(PrefKeys.UnlockedEndings).Split(',').ToList();
            PlayerPrefs.DeleteKey(PrefKeys.UnlockedEndings);
            return true;
        }

        private bool GetLegacyMilestones(ref List<string> list)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.Milestones))
            {
                list = null;
                return false;
            }

            list = PlayerPrefs.GetString(PrefKeys.Milestones).Split(',').ToList();
            PlayerPrefs.DeleteKey(PrefKeys.Milestones);
            return true;
        }

        private bool GetLegacyRound(ref int round)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.UnlockedRecipes))
            {
                round = 0;
                return false;
            }
            
            round = PlayerPrefs.GetInt(PrefKeys.CurrentRound);
            PlayerPrefs.DeleteKey(PrefKeys.CurrentRound);

            return true;
        }

        private bool GetLegacyCovenIntroShown(ref bool isShown)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.CovenIntroShown))
            {
                isShown = false;
                return false;
            }
            
            isShown = PlayerPrefs.GetInt(PrefKeys.CovenIntroShown) == 1;
            PlayerPrefs.DeleteKey(PrefKeys.CovenIntroShown);

            return true;
        }
        private bool GetLegacyAutoCooking(ref bool isUnlocked)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.IsAutoCookingUnlocked))
            {
                isUnlocked = false;
                return false;
            }
            
            isUnlocked = PlayerPrefs.GetInt(PrefKeys.IsAutoCookingUnlocked) == 1;
            PlayerPrefs.DeleteKey(PrefKeys.IsAutoCookingUnlocked);

            return true;
        }

        public void Reset()
        {
            progress = new PlayerProgress();
            SaveProgress();
        }
    }
}