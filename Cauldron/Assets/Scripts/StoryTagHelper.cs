using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
    public static class StoryTagHelper
    {
        public static void SaveMilestone(string tag)
        {
            SaveTag(tag, PrefKeys.Milestones);
        }

        public static void SaveFreeze(string tag)
        {
            SaveTag(tag, PrefKeys.Freezes);
        }

        private static void SaveTag(string tag, string prefKey)
        {
            StringListWrapper tags;
            if (PlayerPrefs.HasKey(prefKey))
            {
                var encodedTags = PlayerPrefs.GetString(prefKey);
                tags = JsonUtility.FromJson<StringListWrapper>(encodedTags);
            }
            else
            {
                tags = new StringListWrapper();
            }

            if (!tags.list.Contains(tag))
            {
                tags.list.Add(tag);
                PlayerPrefs.SetString(prefKey, JsonUtility.ToJson(tags));
                Debug.Log($"[Prefs save: {prefKey}] {tags}");
            }
        }

        public static List<string> GetMilestones()
        {
            if (PlayerPrefs.HasKey(PrefKeys.Milestones))
            {
                RunCompatibilityUpdate();
                
                var encodedTags = PlayerPrefs.GetString(PrefKeys.Milestones);
                var milestones = JsonUtility.FromJson<StringListWrapper>(encodedTags).list;
                return milestones;
            }
            return new List<string>();
        }
        
        public static List<string> GetFreezes()
        {
            if (PlayerPrefs.HasKey(PrefKeys.Freezes))
            {
                var encodedTags = PlayerPrefs.GetString(PrefKeys.Freezes);
                var cooldowns = JsonUtility.FromJson<StringListWrapper>(encodedTags).list;
                Debug.Log($"[Prefs load: {PrefKeys.Freezes}] {encodedTags}");
                return cooldowns;
            }
            return new List<string>();
        }

        private static void RunCompatibilityUpdate()
        {
            if (RemoveMilestone("bishops sister cured"))
            {
                Debug.Log("[Compatibility] Removed 'bishops sister cured' milestone");
            }
        }

        public static bool RemoveMilestone(string tag)
        {
            return RemoveTag(tag, PrefKeys.Milestones);
        }
        
        public static bool RemoveFreeze(string tag)
        {
            return RemoveTag(tag, PrefKeys.Freezes);
        }

        private static bool RemoveTag(string tag, string prefKey)
        {
            if (!PlayerPrefs.HasKey(prefKey))
            {
                return false;
            }

            var encodedTags = PlayerPrefs.GetString(prefKey);
            var tags = JsonUtility.FromJson<StringListWrapper>(encodedTags);
            if (tags.list.Remove(tag))
            {
                PlayerPrefs.SetString(prefKey, JsonUtility.ToJson(tags));
                Debug.Log($"[Prefs remove: {prefKey}] {tag}");
                return true;
            }

            return false;
        }

        public static bool Check(Encounter card, GameDataHandler gameDataHandler)
        {
            return Check(card.requiredStoryTag, gameDataHandler);
        }
        
        public static bool Check(string tagLine, GameDataHandler gameDataHandler)
        {
            if (tagLine is null || string.IsNullOrWhiteSpace(tagLine.Trim()))
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
            return gameDataHandler.storyTags.Contains("circle quest") && !CovenSavingsEnabled(gameDataHandler) && !CovenFeatureUnlocked(gameDataHandler);
        }

        public static bool CovenFeatureUnlocked(GameDataHandler gameDataHandler)
        {
            return gameDataHandler.storyTags.Contains("circle");
        }
    }
}