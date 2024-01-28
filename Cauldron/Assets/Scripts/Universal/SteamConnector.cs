using System;
using System.IO;
using Steamworks;
using UnityEngine;

namespace Universal
{
    public class SteamConnector: MonoBehaviour
    {
        public static bool LoggedIn;
        private void Awake()
        {
            try
            {
                SteamClient.Init(2251010);
                Debug.Log("Steam connected to "+SteamClient.Name);
                LoggedIn = true;
                ExportAchievements();
            }
            catch (Exception e)
            {
                Debug.Log("Steam failed to connect: "+e.Message);
                throw;
            }
        }

        private void OnApplicationQuit()
        {
            SteamClient.Shutdown();
        }

        private void ExportAchievements()
        {
            var file = File.CreateText(Application.dataPath + "/Localize/Achievements_ru.csv");
            file.WriteLine("id;name;description");
            foreach (var a in SteamUserStats.Achievements)
            {
                a.Clear();
                file.WriteLine(a.Identifier + ";" + a.Name + ";" + a.Description);
            }
            file.Close();
        }
    }
}