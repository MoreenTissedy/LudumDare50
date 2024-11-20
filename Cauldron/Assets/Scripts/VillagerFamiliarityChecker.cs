using System.Collections.Generic;
using CauldronCodebase;
using UnityEngine;

public class VillagerFamiliarityChecker
{
    private List<string> unlocked;
    private List<string> locked;
    private IAchievementManager achievements;
    
    private readonly string fileName = "UnlockedVillager";
    private FileDataHandler<ListToSave<string>> fileDataHandler;

    public int FamiliarsCount => unlocked.Count;

    private SODictionary soDictionary;

    public void Init(IAchievementManager achievements, SODictionary soDictionary)
    {
        this.achievements = achievements;
        this.soDictionary = soDictionary;

        fileDataHandler  = new FileDataHandler<ListToSave<string>>(fileName);

        Update();        
    }

    public void Update()
    {
        unlocked = LoadUnlockedVillager();

        locked = new List<string>();
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

        achievements.SetStat("visitors", FamiliarsCount);
        
        Debug.Log($"Visitor add: {tag}. Count unfamiliars is {locked.Count}");
    }

    private List<string> LoadUnlockedVillager()
    {
        return fileDataHandler.IsFileValid() ? fileDataHandler.Load().list : new List<string>();
    }

    private void Save()
    {
        fileDataHandler.Save(new ListToSave<string>(unlocked));
    }
}
