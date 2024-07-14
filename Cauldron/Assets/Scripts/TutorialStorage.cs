﻿using System;
using System.Collections.Generic;
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

public class TutorialStorage
{
    private readonly string fileName = "Tutorial";

    private HashSet<TutorialKeys> FromStringList(StringListWrapper data)
    {
        var tutorials = new HashSet<TutorialKeys>();
        foreach (var item in data.list)
        {
            if(Enum.TryParse(item, out TutorialKeys key))
            {
                tutorials.Add(key);
            }
        }
        return tutorials;
    }

    private StringListWrapper ToStringList(HashSet<TutorialKeys> tutorials)
    {
        var data = new StringListWrapper();
        foreach (var item in tutorials)
        {
            data.list.Add(item.ToString());
        }
        return data;
    }

    public void SaveTutorial(TutorialKeys key)
    {
        FileDataHandler<StringListWrapper> fileDataHandler = new FileDataHandler<StringListWrapper>(fileName);
        HashSet<TutorialKeys> tutorials;

        if (fileDataHandler.IsFileValid())
        {
            tutorials = FromStringList(fileDataHandler.Load());
        }
        else
        {
            tutorials = new HashSet<TutorialKeys>();
        }

        tutorials.Add(key);
        Debug.Log($"tutorial saved: {key}");
        fileDataHandler.Save(ToStringList(tutorials));
    }

    public HashSet<TutorialKeys> GetTutorials()
    {
        if (TryLoadLegacy(out var tutorials))
        {
            return tutorials;
        }
        FileDataHandler<StringListWrapper> fileDataHandler = new FileDataHandler<StringListWrapper>(fileName);
        return fileDataHandler.IsFileValid() ? FromStringList(fileDataHandler.Load()) : new HashSet<TutorialKeys>();
    }

    public bool TryLoadLegacy(out HashSet<TutorialKeys> legacyTutorials)
    {
        HashSet<TutorialKeys> tutorials = new HashSet<TutorialKeys>();
        var hasLegacy = false;

        foreach (TutorialKeys tutorialKey in TutorialKeys.GetValues(typeof(TutorialKeys)))
        {
            var key = tutorialKey.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                tutorials.Add(tutorialKey);
                PlayerPrefs.DeleteKey(key);
                hasLegacy = true;
            }
        }

        FileDataHandler<StringListWrapper> fileDataHandler = new FileDataHandler<StringListWrapper>(fileName);
        if (hasLegacy)
        {
            fileDataHandler.Save(ToStringList(tutorials));
            {
                legacyTutorials = tutorials;
                return true;
            }
        }        
        legacyTutorials = null;
        return false;        
    }
}