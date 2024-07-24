using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using Save;
using UnityEngine;
using Zenject;

[Serializable]
public class PlayerProgress
{
    public List<int> UnlockedRecipes = new List<int>();
    public List<string> UnlockedEndings = new List<string>();
    public List<string> Milestones = new List<string>();
    public int CurrentRound = 0;
    //public bool CovenIntroShown;
    //public bool IsAutoCookingUnlocked;
}

[CreateAssetMenu]
public class PlayerProgressProvider : ScriptableObject
{
    private readonly string fileName = "PlayerProgress";
    private FileDataHandler<PlayerProgress> fileDataHandler;
    
    [SerializeField] public PlayerProgress progress;

    public List<int> GetUnlockedRecipes() => GetPlayerProgress().UnlockedRecipes;
    public List<string> GetUnlockedEndings() => GetPlayerProgress().UnlockedEndings;
    public List<string> GetMilestones() => GetPlayerProgress().Milestones;
    public int CurrentRound => progress.CurrentRound;

    public Action onChangeMilestone;
   
    private void TryInitFileDataHandler()
    {
        if (fileDataHandler != null) return;

        fileDataHandler  = new FileDataHandler<PlayerProgress>(fileName);
    }
    
    public void LoadProgress()
    {
        progress = GetPlayerProgress();
    }

    public void SaveRecipes(List<int> recipes)
    {
        progress.UnlockedRecipes = recipes;
        Debug.Log($"recipes saved");

        SaveProgress();
    }

    public void SaveEndings(List<string> endings)
    {
        progress.UnlockedEndings = endings;
        Debug.Log($"endings saved");

        SaveProgress();
    }

    public void SaveMilestones(List<string> milestones)
    {
        progress.Milestones = milestones;
        Debug.Log($"milestones saved");

        SaveProgress();
        onChangeMilestone?.Invoke();
    }

    public void SaveCurrentRound(int round)
    {
        progress.CurrentRound = round;
        Debug.Log($"milestones saved");

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

        hasLegacy |= GetLegacyRecipes(out legacyProgress.UnlockedRecipes);
        hasLegacy |= GetLegacyEndings(out legacyProgress.UnlockedEndings);        
        hasLegacy |= GetLegacyMilestones(out legacyProgress.Milestones);        
        hasLegacy |= GetLegacyRound(out legacyProgress.CurrentRound);

        TryInitFileDataHandler();
        if (hasLegacy)
        {
            fileDataHandler.Save(legacyProgress);
            return true;
        }        
        legacyProgress = null;
        return false;    
    }

    private bool GetLegacyRecipes(out List<int> list)
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

    private bool GetLegacyEndings(out List<string> list)
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

    private bool GetLegacyMilestones(out List<string> list)
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

    private bool GetLegacyRound(out int round)
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
}