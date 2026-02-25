using System.Collections.Generic;
using UnityEngine;
using Universal;

namespace CauldronCodebase
{
    public static class Freezes
    {
        public static bool RemoveFreeze(string tag)
        {
            return RemoveTag(tag, PrefKeys.Freezes);
        }

        private static bool RemoveTag(string tag, string prefKey)
        {
            if (!PlayerPrefsService.HasKey(prefKey))
            {
                return false;
            }

            var encodedTags = PlayerPrefsService.GetString(prefKey);
            var tags = JsonUtility.FromJson<StringListWrapper>(encodedTags);
            if (tags.list.Remove(tag))
            {
                PlayerPrefsService.SetString(prefKey, JsonUtility.ToJson(tags));
                Debug.Log($"[Prefs remove: {prefKey}] {tag}");
                return true;
            }

            return false;
        }
        
        public static void SaveFreeze(string tag)
        {
            SaveTag(tag, PrefKeys.Freezes);
        }

        private static void SaveTag(string tag, string prefKey)
        {
            StringListWrapper tags;
            if (PlayerPrefsService.HasKey(prefKey))
            {
                var encodedTags = PlayerPrefsService.GetString(prefKey);
                tags = JsonUtility.FromJson<StringListWrapper>(encodedTags);
            }
            else
            {
                tags = new StringListWrapper();
            }

            if (!tags.list.Contains(tag))
            {
                tags.list.Add(tag);
                PlayerPrefsService.SetString(prefKey, JsonUtility.ToJson(tags));
                Debug.Log($"[Prefs save: {prefKey}] {tags}");
            }
        }

        public static List<string> GetFreezes()
        {
            if (PlayerPrefsService.HasKey(PrefKeys.Freezes))
            {
                var encodedTags = PlayerPrefsService.GetString(PrefKeys.Freezes);
                return JsonUtility.FromJson<StringListWrapper>(encodedTags).list;
            }
            return new List<string>();
        }
    }
}