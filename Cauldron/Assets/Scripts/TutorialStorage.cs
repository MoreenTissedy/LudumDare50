using System;
using Save;
using UnityEngine;

public enum TutorialKeys
{
    TUTORIAL_BOOK_OPENED,
    BOOK_AUTOCOOKING_OPENED,
    TUTORIAL_VISITOR,
    TUTORIAL_CHANGE_SCALE,
    TUTORIAL_POTION_DENIED,
    TUTORIAL_RECIPE_HINTS 
}

[Serializable]
public class Tutorials
{
    public bool TUTORIAL_BOOK_OPENED = false;
    public bool BOOK_AUTOCOOKING_OPENED = false;
    public bool TUTORIAL_VISITOR = false;
    public bool TUTORIAL_CHANGE_SCALE = false;
    public bool TUTORIAL_POTION_DENIED = false;
    public bool TUTORIAL_RECIPE_HINTS = false;

    public void SetValue(TutorialKeys key, bool value)
    {
        switch (key){
            case TutorialKeys.TUTORIAL_BOOK_OPENED:
                TUTORIAL_BOOK_OPENED = value;
                break;
            case TutorialKeys.BOOK_AUTOCOOKING_OPENED:
                BOOK_AUTOCOOKING_OPENED = value;
                break;
            case TutorialKeys.TUTORIAL_VISITOR:
                TUTORIAL_VISITOR = value;
                break;
            case TutorialKeys.TUTORIAL_CHANGE_SCALE:
                TUTORIAL_CHANGE_SCALE = value;
                break;
            case TutorialKeys.TUTORIAL_POTION_DENIED:
                TUTORIAL_POTION_DENIED = value;
                break;
            case TutorialKeys.TUTORIAL_RECIPE_HINTS:
                TUTORIAL_RECIPE_HINTS = value;
                break;
        }
    }

    public bool GetValue(TutorialKeys key, out bool value)
    {
        switch (key){
            case TutorialKeys.TUTORIAL_BOOK_OPENED:
                value = TUTORIAL_BOOK_OPENED;
                return true;
            case TutorialKeys.BOOK_AUTOCOOKING_OPENED:
                value = BOOK_AUTOCOOKING_OPENED;
                return true;
            case TutorialKeys.TUTORIAL_VISITOR:
                value = TUTORIAL_VISITOR;
                return true;
            case TutorialKeys.TUTORIAL_CHANGE_SCALE:
                value = TUTORIAL_CHANGE_SCALE;
                return true;
            case TutorialKeys.TUTORIAL_POTION_DENIED:
                value = TUTORIAL_POTION_DENIED;
                return true;
            case TutorialKeys.TUTORIAL_RECIPE_HINTS:
                value = TUTORIAL_RECIPE_HINTS;
                return true;
        }
        value = false;
        return false;
    }
}

[CreateAssetMenu()]
public class TutorialStorage: ScriptableObject
{
    private readonly string fileName = "Tutorial";

    public void SaveTutorial(TutorialKeys key, bool value)
    {
        FileDataHandler<Tutorials> fileDataHandler = new FileDataHandler<Tutorials>(fileName);
        Tutorials tutorials;

        if (fileDataHandler.IsFileValid())
        {
            tutorials = fileDataHandler.Load();
        }
        else
        {
            tutorials = new Tutorials();
        }

        tutorials.SetValue(key, value);
        Debug.Log($"tutorial saved: {key}:{value} ");
        fileDataHandler.Save(tutorials);
    }

    public bool TryGetTutorial(TutorialKeys key, out bool value)
    {
        var tutorials = GetTutorials();
        if (tutorials.GetValue(key, out value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Tutorials GetTutorials()
    {
        if (TryLoadLegacy(out var tutorials))
        {
            return tutorials;
        }
        FileDataHandler<Tutorials> fileDataHandler = new FileDataHandler<Tutorials>(fileName);
        return fileDataHandler.IsFileValid() ? fileDataHandler.Load() : new Tutorials();
    }

    public bool TryLoadLegacy(out Tutorials legacyTutorials)
    {
        Tutorials tutorials = new Tutorials();
        var hasLegacy = false;

        foreach (TutorialKeys tutorialKey in TutorialKeys.GetValues(typeof(TutorialKeys)))
        {
            var key = tutorialKey.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                var value = PlayerPrefs.GetInt(key) == 1;
                tutorials.SetValue(tutorialKey, value);
                PlayerPrefs.DeleteKey(key);
                hasLegacy = true;
            }
        }

        FileDataHandler<Tutorials> fileDataHandler = new FileDataHandler<Tutorials>(fileName);
        if (hasLegacy)
        {
            fileDataHandler.Save(tutorials);
            {
                legacyTutorials = tutorials;
                return true;
            }
        }        
        legacyTutorials = null;
        return false;        
    }
}
