namespace Universal
{
    public static class PlayerPrefsService
    {
        public static bool HasKey(string key) => UnityEngine.PlayerPrefs.HasKey(key);

        public static void SetInt(string key, int value, bool save = true)
        {
            UnityEngine.PlayerPrefs.SetInt(key, value);
            if (save)
            {
                Save();
            }
        }

        public static void SetBool(string key, bool value, bool save = true)
        {
            UnityEngine.PlayerPrefs.SetInt(key, value ? 1 : 0);
            if (save)
            {
                Save();
            }
        }

        public static void SetFloat(string key, float value, bool save = true)
        {
            UnityEngine.PlayerPrefs.SetFloat(key, value);
            if (save)
            {
                Save();
            }
        }

        public static void SetString(string key, string value, bool save = true)
        {
            UnityEngine.PlayerPrefs.SetString(key, value);
            if (save)
            {
                Save();
            }
        }

        public static int GetInt(string key, int defaultValue = 0) => UnityEngine.PlayerPrefs.GetInt(key, defaultValue);

        public static bool GetBool(string key, bool defaultValue = false) =>
            UnityEngine.PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;

        public static float GetFloat(string key, float defaultValue = 0f) =>
            UnityEngine.PlayerPrefs.GetFloat(key, defaultValue);

        public static string GetString(string key, string defaultValue = "") =>
            UnityEngine.PlayerPrefs.GetString(key, defaultValue);

        public static void Save()
        {
            UnityEngine.PlayerPrefs.Save();
        }

        public static void DeleteKey(string key)
        {
            UnityEngine.PlayerPrefs.DeleteKey(key);
            Save();
        }

        public static void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
            Save();
        }
    }
}