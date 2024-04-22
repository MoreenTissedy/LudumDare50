using System;
using System.IO;
using UnityEngine;

namespace Save
{
    public class FileDataHandler<T> where T: class
    {
        private readonly string fullPath;

        public FileDataHandler(string dataFileName)
        {
            string dataDirPath = Application.persistentDataPath;
            fullPath = Path.Combine(dataDirPath, dataFileName);
        }

        public bool IsFileValid()
        {
            return File.Exists(fullPath);
        }

        public T Load()
        {
            T loadedData = null;
            if (IsFileValid())
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

                    loadedData = JsonUtility.FromJson<T>(dataToLoad);
                    Debug.LogWarning($"Data loaded from {fullPath}");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured  when trying to load data from file: " + fullPath + "\n" + e);
                }
            }

            return loadedData;
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