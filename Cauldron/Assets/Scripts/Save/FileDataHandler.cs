using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using nn;
using UnityEngine;

namespace CauldronCodebase
{
    public static class SwitchFileHelper 
    {
        public static readonly List<string> Paths = new List<string>();
        
        public static bool FileValid(string fullPath)
        {
            nn.fs.EntryType entryType = 0;
            nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, fullPath);
            return !nn.fs.FileSystem.ResultPathNotFound.Includes(result);
        }

        public static void DeleteFile(string path)
        {
            nn.fs.File.Delete(path);
        }
    }
    
    public class FileDataHandler<T> where T : class
    {
        private readonly string fullPath;

#if UNITY_SWITCH
        public const string mountName = "Saves";
        private const int saveDataSize = 32;
        private nn.fs.FileHandle fileHandle;
#endif

        public FileDataHandler(string dataFileName, bool extension = true)
        {
            if (extension)
            {
                dataFileName += ".sav";
            }
#if UNITY_SWITCH
            fullPath = string.Format("{0}:/{1}", mountName, dataFileName);
            SwitchFileHelper.Paths.Add(fullPath);
#else
            string dataDirPath = Application.persistentDataPath;
            string SubFolder = "Saves";
            string subDirPath = Path.Combine(dataDirPath, SubFolder);
            if (!Directory.Exists(subDirPath))
            {
                Directory.CreateDirectory(subDirPath);
            }
            fullPath = Path.Combine(subDirPath, dataFileName);
#endif
        }

        public bool IsFileValid()
        {
#if UNITY_SWITCH
            return SwitchFileHelper.FileValid(fullPath);
#else
            return File.Exists(fullPath);
#endif
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
                    Debug.LogError("Error occured  when trying to load data from file to a Unity object: " + fullPath +
                                   "\n" + e);
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
#if UNITY_SWITCH
            return LoadForSwitch();
#else
            string dataToLoad;

            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    dataToLoad = reader.ReadToEnd();
                }
            }

            return dataToLoad;
#endif
        }

        public void Save(T data)
        {
            try
            {
#if UNITY_SWITCH
                if (!IsFileValid())
                {
                    var result = nn.fs.File.Create(fullPath, saveDataSize);
                    result.abortUnlessSuccess();
                }
#else
                string directoryName = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
#endif

                string dataToStore = JsonUtility.ToJson(data, true);
#if UNITY_SWITCH
                SaveForSwitch(dataToStore);
#else
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured  when trying to save data to file: " + fullPath + "\n" + e);
            }
        }

        public void Delete()
        {
#if UNITY_SWITCH
            SwitchFileHelper.DeleteFile(fullPath);
#else
            File.Delete(fullPath);
#endif
        }

        private void SaveForSwitch(string stringData)
        {
            byte[] data;
            using (BinaryWriter writer = new BinaryWriter(new MemoryStream(sizeof(int))))
            {
                writer.Write(stringData);

                writer.BaseStream.Close();
                data = (writer.BaseStream as MemoryStream).GetBuffer();
                Debug.Assert(data.Length == sizeof(int)); //TODO fails - research
            }

#if UNITY_SWITCH
            UnityEngine.Switch.Notification.EnterExitRequestHandlingSection();
#endif

            nn.Result result = nn.fs.File.Open(ref fileHandle, fullPath, nn.fs.OpenFileMode.Write| nn.fs.OpenFileMode.AllowAppend);
            result.abortUnlessSuccess();

            result = nn.fs.File.Write(fileHandle, 0, data, data.LongLength, nn.fs.WriteOption.Flush);
            result.abortUnlessSuccess();

            nn.fs.File.Close(fileHandle);
            result = nn.fs.FileSystem.Commit(mountName);
            result.abortUnlessSuccess();

#if UNITY_SWITCH
            UnityEngine.Switch.Notification.LeaveExitRequestHandlingSection();
#endif
        }

        private string LoadForSwitch()
        {
            Result result;
            if (!SwitchFileHelper.FileValid(fullPath))
            {
                return string.Empty;
            }

            result = nn.fs.File.Open(ref fileHandle, fullPath, nn.fs.OpenFileMode.Read);
            result.abortUnlessSuccess();

            long fileSize = 0;
            result = nn.fs.File.GetSize(ref fileSize, fileHandle);
            result.abortUnlessSuccess();

            byte[] data = new byte[fileSize];
            result = nn.fs.File.Read(fileHandle, 0, data, fileSize);
            result.abortUnlessSuccess();

            nn.fs.File.Close(fileHandle);

            string stringData;

            using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
            {
                stringData = reader.ReadString();
            }

            return stringData;
        }
    }
}