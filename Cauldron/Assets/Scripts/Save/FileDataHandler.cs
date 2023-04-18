using System;
using System.IO;
using UnityEngine;

namespace Save
{
    public class FileDataHandler
    {
        public const string PrefSaveKey = "SaveExists";
        
        private string dataDirPath;
        private string dataFileName;
        private string fullPath;

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
            fullPath = Path.Combine(dataDirPath, dataFileName);
        }

        public GameData Load()
        {
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad;
                    
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                    Debug.LogWarning($"Data loaded from {fullPath}");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured  when trying to load data from file: " + fullPath + "\n" + e);
                }
            }

            return loadedData;
        }

        public void Save(GameData data)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                        PlayerPrefs.SetInt(PrefSaveKey, 1);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured  when trying to save data to file: " + fullPath + "\n" + e);
            }
        }

        public void Delete()
        {
            File.Delete(fullPath);
            PlayerPrefs.DeleteKey(PrefSaveKey);
        }
    }
}