using System;
using System.Linq;
using Steamworks;
using UnityEngine;

namespace Universal
{
    public class SteamConnector: MonoBehaviour
    {
        private void Awake()
        {
            SteamClient.Init(2251010);
            Debug.Log("Steam connected to "+SteamClient.Name);
        }

        private void OnApplicationQuit()
        {
            SteamClient.Shutdown();
        }
    }
}