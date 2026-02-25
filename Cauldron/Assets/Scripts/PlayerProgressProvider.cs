using System;
using UnityEngine;
using Universal;

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
            Update();
        }
        
        public void Update()
        {
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
            if (!PlayerPrefsService.HasKey(PrefKeys.CurrentRound))
            {
                round = 0;
                return false;
            }
            
            round = PlayerPrefsService.GetInt(PrefKeys.CurrentRound);
            PlayerPrefsService.DeleteKey(PrefKeys.CurrentRound);

            return true;
        }

        private bool GetLegacyCovenIntroShown(ref bool isShown)
        {
            if (!PlayerPrefsService.HasKey(PrefKeys.CovenIntroShown))
            {
                isShown = false;
                return false;
            }
            
            isShown = PlayerPrefsService.GetInt(PrefKeys.CovenIntroShown) == 1;
            PlayerPrefsService.DeleteKey(PrefKeys.CovenIntroShown);

            return true;
        }
        private bool GetLegacyAutoCooking(ref bool isUnlocked)
        {
            if (!PlayerPrefsService.HasKey(PrefKeys.IsAutoCookingUnlocked))
            {
                isUnlocked = false;
                return false;
            }
            
            isUnlocked = PlayerPrefsService.GetInt(PrefKeys.IsAutoCookingUnlocked) == 1;
            PlayerPrefsService.DeleteKey(PrefKeys.IsAutoCookingUnlocked);

            return true;
        }
    }
}