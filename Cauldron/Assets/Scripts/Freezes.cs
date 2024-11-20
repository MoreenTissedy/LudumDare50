using System.Collections.Generic;
using UnityEngine;

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

        public static List<string> GetFreezes()
        {
            if (PlayerPrefs.HasKey(PrefKeys.Freezes))
            {
                var encodedTags = PlayerPrefs.GetString(PrefKeys.Freezes);
                return JsonUtility.FromJson<StringListWrapper>(encodedTags).list;
            }
            return new List<string>();
        }
    }
}