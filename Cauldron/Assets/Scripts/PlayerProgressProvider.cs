using System;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using Save;
using UnityEngine;

[Serializable]
public class PlayerProgress
{
    public List<int> UnlockedRecipes = new List<int>();
    public List<string> UnlockedEndings = new List<string>();
    public List<string> Milestones;
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
    public List<string> UnlockedEndings => progress.UnlockedEndings;
    public List<string> GetMilestones() => GetPlayerProgress().Milestones;

    public Action onChangeMilestone;
   
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
}