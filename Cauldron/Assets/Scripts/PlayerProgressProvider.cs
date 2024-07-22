using System;
using System.Collections.Generic;
using CauldronCodebase;
using Save;
using UnityEngine;

[Serializable]
public class PlayerProgress
{
    public List<int> UnlockedRecipes = new List<int>();
    //public List<string> UnlockedEndings;
    //public List<string> Milestones;
    //public int CurrentRound;
    //public bool CovenIntroShown;
    //public bool IsAutoCookingUnlocked;
}

[CreateAssetMenu]
public class PlayerProgressProvider : ScriptableObject
{
    private readonly string fileName = "PlayerProgress";
    private FileDataHandler<PlayerProgress> fileDataHandler;
    
    [SerializeField] public PlayerProgress progress;

    public List<int> UnlockedRecipes => progress.UnlockedRecipes;
   
    private void TryInitFileDataHandler()
    {
        if (fileDataHandler != null) return;

        fileDataHandler  = new FileDataHandler<PlayerProgress>(fileName);
    }
    
    public void Init()
    {
        progress = GetPlayerProgress();
    }

    public void SaveRecipes(List<int> recipes)
    {
        progress.UnlockedRecipes = recipes;
        Debug.Log($"recipes saved");

        SaveProgress();
    }

    private void SaveProgress()
    {
        TryInitFileDataHandler();
        fileDataHandler.Save(progress);
    }

    private PlayerProgress GetPlayerProgress()
    {
        if (TryLoadLegacy(out var legacyProgress))
        {
            return legacyProgress;
        }
        TryInitFileDataHandler();
        return fileDataHandler.IsFileValid() ? fileDataHandler.Load() : new PlayerProgress();
    }

    private bool TryLoadLegacy(out PlayerProgress legacyProgress)
    {
        legacyProgress = new PlayerProgress();
        bool hasLegacy = false;

        if (PlayerPrefs.HasKey(PrefKeys.UnlockedRecipes))
        {
            legacyProgress.UnlockedRecipes = GetLegacyRecipes();
            hasLegacy = true;
        }

        TryInitFileDataHandler();
        if (hasLegacy)
        {
            fileDataHandler.Save(legacyProgress);
            {
                return true;
            }
        }        
        legacyProgress = null;
        return false;    
    }

    private List<int> GetLegacyRecipes()
    {
        List<int> list = new List<int>();
        string data = PlayerPrefs.GetString(PrefKeys.UnlockedRecipes);
        foreach (var potion in data.Split(','))
        {
            if (string.IsNullOrWhiteSpace(potion))
            {
                continue;
            }
            list.Add(int.Parse(potion));
        }
        return list;
    }
}
