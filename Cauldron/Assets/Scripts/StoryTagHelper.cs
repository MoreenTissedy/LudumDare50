using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

namespace CauldronCodebase
{
    public static class StoryTagHelper
    {
        public static void SaveMilestone(string tag)
        {
            StringListWrapper tags;
            if (PlayerPrefs.HasKey(PrefKeys.Milestones))
            {
                var encodedTags = PlayerPrefs.GetString(PrefKeys.Milestones);
                tags = JsonUtility.FromJson<StringListWrapper>(encodedTags);
            }
            else
            {
                tags = new StringListWrapper();
            }
            if (!tags.list.Contains(tag))
            {
                tags.list.Add(tag);
                PlayerPrefs.SetString(PrefKeys.Milestones, JsonUtility.ToJson(tags));
            }
        }

        public static List<string> GetMilestones()
        {
            if (PlayerPrefs.HasKey(PrefKeys.Milestones))
            {
                var encodedTags = PlayerPrefs.GetString(PrefKeys.Milestones);
                return JsonUtility.FromJson<StringListWrapper>(encodedTags).list;
            }
            return new List<string>();
        }
        
        public static void RemoveMilestone(string tag)
        {
            if (!PlayerPrefs.HasKey(PrefKeys.Milestones))
            {
                return;
            }
            var encodedTags = PlayerPrefs.GetString(PrefKeys.Milestones);
            var tags = JsonUtility.FromJson<StringListWrapper>(encodedTags);
            tags.list.Remove(tag);
            PlayerPrefs.SetString(PrefKeys.Milestones, JsonUtility.ToJson(tags));
        }
        
        public static bool Check(Encounter card, GameDataHandler gameDataHandler)
        {
            return Check(card.requiredStoryTag, gameDataHandler);
        }
        
        public static bool Check(string tagLine, GameDataHandler gameDataHandler)
        {
            if (string.IsNullOrWhiteSpace(tagLine.Trim()))
            {
                return true;
            }

            string[] tags = tagLine.Split(',');
            bool valid = true;
            foreach (var tag in tags)
            {
                string trim = tag.Trim();
                if (trim == EndingsProvider.LOW_FAME || trim == EndingsProvider.LOW_FEAR ||
                    trim == EndingsProvider.HIGH_FAME || trim == EndingsProvider.HIGH_FEAR)
                {
                    continue;
                }
                if (trim.StartsWith("!"))
                {
                    valid = valid && !gameDataHandler.storyTags.Contains(trim.TrimStart('!'));
                }
                else
                {
                    valid = valid && gameDataHandler.storyTags.Contains(trim);
                }
            }
            return valid;
        }

        public static bool CovenSavingsEnabled(GameDataHandler gameDataHandler)
        {
            return gameDataHandler.storyTags.Contains("circle money") && !CovenFeatureUnlocked(gameDataHandler);
        }
        
        public static bool CovenQuestEnabled(GameDataHandler gameDataHandler)
        {
            return gameDataHandler.storyTags.Contains("circle quest") && !CovenSavingsEnabled(gameDataHandler);
        }

        public static bool CovenFeatureUnlocked(GameDataHandler gameDataHandler)
        {
            return gameDataHandler.storyTags.Contains("circle");
        }
    }
}