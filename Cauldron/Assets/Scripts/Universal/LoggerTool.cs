using System;
using System.IO;
using UnityEngine;

namespace Universal
{
    public class LoggerTool : MonoBehaviour
    {
        public static LoggerTool TheOne;
        
        private StreamWriter stream;
        private string userId = "X";
        private float timer;
        private float levelTimer;

        private void Awake()
        {
            TheOne = this;

            levelTimer = Time.time;
            GetUserId();
            OpenFile();
        }

        private void GetUserId()
        {
            if (PlayerPrefs.HasKey("User"))
            {
                userId = PlayerPrefs.GetString("User");
            }
            else
            {
                userId = Guid.NewGuid().ToString("N");
                PlayerPrefs.SetString("User", userId);
            }
        }

        private void OpenFile()
        {
            var path = $"{userId}-{System.DateTime.Now.ToString("M-d-HHmm")}.txt";
            stream = new StreamWriter(new FileStream(path, FileMode.Create));
        }

        public void StartTimer()
        {
            timer = Time.timeSinceLevelLoad;
        }

        public void LogTimer()
        {
            Log($"T: {(Time.timeSinceLevelLoad - timer).ToString("F1")}");
        }

        public void Log(string line)
        {
            stream.WriteLine(line);
        }

        private void OnDestroy()
        {
            Log("Session time in secs: "+(Time.time - levelTimer));
            stream.Close();
        }
    }
}