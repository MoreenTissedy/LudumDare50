using System;
using Steamworks;
using UnityEngine;

namespace Universal
{
    public class SteamConnector: MonoBehaviour
    {
        public static bool LoggedIn = true;
        public static bool HasPremium = false;
        private void Awake()
        {
            try
            {
                SteamClient.Init(2027434);
                Debug.Log("Steam connected to "+SteamClient.Name);
                LoggedIn = true;
                //HasPremium = SteamApps.IsSubscribedToApp(2955860);
                SteamUserStats.RequestCurrentStats();
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