using System.Collections.Generic;
using CauldronCodebase;
using UnityEngine;

public class VisitorsProvider
{
    private static readonly int CHARACTER_COUNT = 54;
    private List<string> unlocked;
    private IAchievementManager achievements;
    
    private readonly string fileName = "UnlockedVisitors";
    private FileDataHandler<ListToSave<string>> fileDataHandler;

    public void Init(IAchievementManager achievements)
    {
        this.achievements = achievements;

        fileDataHandler  = new FileDataHandler<ListToSave<string>>(fileName);
        unlocked = LoadUnlockedVisitors();
    }

    public void TryAddVisitor(string tag)
    {
        if (unlocked.Contains(tag)) return;
        
        unlocked.Add(tag);
        Save();        

        Debug.Log($"Visitor add: {tag}. Count is {unlocked.Count}");
        
        if (unlocked.Count == CHARACTER_COUNT)
        {
            achievements.TryUnlock(AchievIdents.VISITORS_ALL);
            Debug.Log("ACHIEVEMENT ALL_VISITORS GET!");
        }
    }

    private List<string> LoadUnlockedVisitors()
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
