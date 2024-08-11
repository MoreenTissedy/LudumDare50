using System;
using System.IO;
using UnityEngine;

namespace CauldronCodebase
{
    public class FileDataHandler<T> where T: class
    {
        private readonly string fullPath;

        public FileDataHandler(string dataFileName, bool extension = true)
        {
            string dataDirPath = Application.persistentDataPath;
            if (extension)
            {
                dataFileName += ".sav";
            }
            fullPath = Path.Combine(dataDirPath, "Saves", dataFileName);
        }

        public bool IsFileValid()
        {
            return File.Exists(fullPath);
        }
        
        public T LoadWithOverwrite(T unityObject)
        {
            if (IsFileValid())
            {
                try
                {
                    var dataToLoad = GetFileData();
                    JsonUtility.FromJsonOverwrite(dataToLoad, unityObject);
                    Debug.Log($"Data loaded from {fullPath} to Unity object");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured  when trying to load data from file to a Unity object: " + fullPath + "\n" + e);
                }
            }

            return unityObject;
        }

        public T Load()
        {
            T loadedData = null;
            if (IsFileValid())
            {
                try
                {
                    var dataToLoad = GetFileData();
                    loadedData = JsonUtility.FromJson<T>(dataToLoad);
                    Debug.Log($"Data loaded from {fullPath}");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured  when trying to load data from file: " + fullPath + "\n" + e);
                }
            }

            return loadedData;
        }

        private string GetFileData()
        {
            string dataToLoad;

            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            return dataToLoad;
        }

        public void Save(T data)
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
        }
    }
}