using System;
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
    }
}