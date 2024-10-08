using System;
using UnityEngine;

namespace CauldronCodebase
{
    [Serializable]
    public class PlayerProgress
    {
        public int CurrentRound = 0;
        public bool CovenIntroShown;
        public bool IsAutoCookingUnlocked;
        public bool WardrobeUnlocked;
    }

    public class PlayerProgressProvider
    {
        private readonly string fileName = "PlayerProgress";
        private FileDataHandler<PlayerProgress> fileDataHandler;
        
        public PlayerProgress progress;
        
        public int CurrentRound => progress.CurrentRound;    
        public bool CovenIntroShown => progress.CovenIntroShown;
        public bool IsAutoCookingUnlocked => progress.IsAutoCookingUnlocked;
        public bool IsWardrobeUnlocked => progress.WardrobeUnlocked;
        
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
        
        public void SaveWardrobeUnlocked()
        {
            progress.WardrobeUnlocked = true;
            Debug.Log($"Wardrobe unlocked saved");

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

        private bool GetLegacyRound(ref int round)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.CurrentRound))
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
    }
}