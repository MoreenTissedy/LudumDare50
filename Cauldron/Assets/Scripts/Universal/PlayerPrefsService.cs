using CauldronCodebase;
using UnityEngine;

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

#if UNITY_SWITCH
            byte[] data = UnityEngine.Switch.PlayerPrefsHelper.rawData;

            UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();

            nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();
            nn.Result result = nn.fs.File.Open(ref fileHandle, filePath,
                nn.fs.OpenFileMode.Write | nn.fs.OpenFileMode.AllowAppend);

            if (result.IsSuccess())
            {
                const long offset = 0;
                result = nn.fs.File.Write(fileHandle, offset, data, data.LongLength, nn.fs.WriteOption.Flush);
                nn.fs.File.Close(fileHandle);

                if (result.IsSuccess())
                {
                    result = nn.fs.FileSystem.Commit(mountName);
                }
                else
                {
                    Debug.LogError("save prefs failed " + result.ToString());
                }
            }

            UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
        }

        public static void Initialize()
        {
#if UNITY_SWITCH
            if (!SwitchFileHelper.Paths.Contains(filePath))
            {
                SwitchFileHelper.Paths.Add(filePath);
            }
            nn.fs.EntryType entryType = 0;
            nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);

            if (result.IsSuccess() && entryType == nn.fs.EntryType.File)
            {
                Load();
            }
            else if (nn.fs.FileSystem.ResultPathNotFound.Includes(result))
            {
                Debug.Log("Creating new player prefs file");

                // This ensures rawData is populated with default values
                UnityEngine.PlayerPrefs.Save();

                byte[] data = UnityEngine.Switch.PlayerPrefsHelper.rawData;

                UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();

                result = nn.fs.File.Create(filePath, data.LongLength);
                if (result.IsSuccess())
                {
                    nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();
                    result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Write);

                    if (result.IsSuccess())
                    {
                        result = nn.fs.File.Write(fileHandle, 0, data, data.LongLength, nn.fs.WriteOption.Flush);
                        nn.fs.File.Close(fileHandle);

                        if (result.IsSuccess())
                        {
                            result = nn.fs.FileSystem.Commit(mountName);
                        }
                    }
                }

                UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
            }
#endif
        }

        public static void Load()
        {
#if UNITY_SWITCH
            nn.fs.EntryType entryType = 0;
            nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath);

            if (result.IsSuccess() && entryType == nn.fs.EntryType.File)
            {
                nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();
                result = nn.fs.File.Open(ref fileHandle, filePath, nn.fs.OpenFileMode.Read);
                if (result.IsSuccess())
                {
                    long fileSize = 0;
                    result = nn.fs.File.GetSize(ref fileSize, fileHandle);
                    if (result.IsSuccess() && fileSize > 0)
                    {
                        byte[] data = new byte[fileSize];
                        result = nn.fs.File.Read(fileHandle, 0, data, fileSize);
                        if (result.IsSuccess())
                        {
                            UnityEngine.Switch.PlayerPrefsHelper.rawData = data;
                        }
                    }

                    nn.fs.File.Close(fileHandle);
                }
            }
#endif
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

        // Switch-specific constants (only compiled for Switch)
#if UNITY_SWITCH
        private const string mountName = "Saves";
        private const string fileName = "PlayerPrefsData";
        private static readonly string filePath = string.Format("{0}:/{1}", mountName, fileName);
#endif
    }
}