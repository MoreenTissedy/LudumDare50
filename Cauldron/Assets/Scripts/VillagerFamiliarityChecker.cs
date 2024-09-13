using System.Collections.Generic;
using CauldronCodebase;
using UnityEngine;

public class VillagerFamiliarityChecker
{
    private List<string> unlocked;
    private List<string> locked = new List<string>();
    private IAchievementManager achievements;
    
    private readonly string fileName = "UnlockedVillager";
    private FileDataHandler<ListToSave<string>> fileDataHandler;

    public void Init(IAchievementManager achievements, SODictionary soDictionary)
    {
        this.achievements = achievements;

        fileDataHandler  = new FileDataHandler<ListToSave<string>>(fileName);
        unlocked = LoadUnlockedVillager();

        foreach(var item in soDictionary.AllScriptableObjects)
        {
            if (item.Value is Villager && !unlocked.Contains(item.Value.name))
            {
                locked.Add(item.Value.name);
            }
        }
    }

    public void TryAddVisitor(string tag)
    {
        if (unlocked.Contains(tag)) return;
        
        unlocked.Add(tag);
        locked.Remove(tag);
        Save();

        Debug.Log($"Visitor add: {tag}. Count unfamiliars is {locked.Count}");
        
        if (locked.Count == 0)
        {
            achievements.TryUnlock(AchievIdents.VISITORS_ALL);
            Debug.Log("ACHIEVEMENT ALL_VISITORS GET!");
        }
    }

    private List<string> LoadUnlockedVillager()
    {
        return fileDataHandler.IsFileValid() ? fileDataHandler.Load().list : new List<string>();
    }

    private void Save()
    {
        fileDataHandler.Save(new ListToSave<string>(unlocked));
    }
    
    public void Reset()
    {
        unlocked.Clear();
        Save();
    }
}
