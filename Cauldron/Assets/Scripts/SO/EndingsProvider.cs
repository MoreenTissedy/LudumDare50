using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Endings Provider", menuName = "Endings Provider", order = 10)]
    public class EndingsProvider : ScriptableObject
    {
        [Flags]
        public enum Unlocks {
            None = 0,
            HighMoney = 1,
            HighFame = 2,
            HighFear = 4,
            LowFame = 8,
            LowFear = 16
        }
        [Tooltip("High money, high fame, high fear, low fame, low fear")]
        [SerializeField] private Ending[] endings;
        [SerializeField] private int threshold = 70;
        [SerializeField] private int lowThreshold = 20;

        private Unlocks unlocked;
        private const string _KEY_ = "Endings";
        public Dictionary<Unlocks, Ending> Endings;

        public void Init()
        {
            Endings = new Dictionary<Unlocks, Ending>(10)
            {
                {Unlocks.HighMoney, endings[0]},
                {Unlocks.HighFame, endings[1]},
                {Unlocks.HighFear, endings[2]},
                {Unlocks.LowFame, endings[3]},
                {Unlocks.LowFear, endings[4]}
            };

            if (PlayerPrefs.HasKey(_KEY_))
            {
                unlocked = (Unlocks)PlayerPrefs.GetInt(_KEY_);
            }
            else
            {
                unlocked = 0;
            }

            foreach (var unlock in Endings.Keys)
            {
                Debug.Log($"ending {unlock} unlocked: {Unlocked(unlock)}");
            }
        }

        public bool Unlocked(Unlocks ending)
        {
            return unlocked.HasFlag(ending);
        }

        public bool Unlocked(int i)
        {
            return ((int)unlocked & (1 << i)) > 0;
        }

        public bool Unlocked(Ending ending)
        {
            foreach (var keyValuePair in Endings)
            {
                if (keyValuePair.Value == ending)
                {
                    return Unlocked(keyValuePair.Key);
                }
            }
            return false;
        }

        public Ending Get(int i)
        {
            int index = 1 << i;
            return Endings[(Unlocks)index];
        }

        public void Unlock(int i)
        {
            unlocked += 1 << i;
            PlayerPrefs.SetInt(_KEY_, (int)unlocked);
        }

        public void Unlock(Unlocks ending)
        {
            unlocked |= ending;
            PlayerPrefs.SetInt(_KEY_, (int)unlocked);
        }
        
        
        public int GetIndexOf(Unlocks ending)
        {
            var intPtr = (int)ending;
            int k = 1;
            int step = 0;
            while (step < Endings.Count)
            {
                if (k == intPtr)
                    return step;
                step++;
                k = k * 2;
            }

            return -1;
        }
         
        //TODO refactor statuses into structs and introduce a separate status checker entity - StatusChecker.Run()
        public void StatusChecks(GameManager gm)
        {
            //endings
            //[Tooltip("High money, high fame, high fear, low fame, low fear")]
            if (gm.GameState.Fame >= gm.Settings.statusBars.Total)
            {
                gm.EndGame(Unlocks.HighFame);
                return;
            }

            if (gm.GameState.Fear >= gm.Settings.statusBars.Total)
            {
                gm.EndGame(Unlocks.HighFear);
                return;
            }

            if (gm.GameState.Money >= gm.Settings.statusBars.Total)
            {
                gm.EndGame(Unlocks.HighMoney);
                return;

            }

            if (gm.GameState.Fame <= 0)
            {
                gm.EndGame(Unlocks.LowFame);
                return;
            }

            if (gm.GameState.Fear <= 0)
            {
                gm.EndGame(Unlocks.LowFear);
                return;
            }

            gm.GameState.CheckStatusesThreshold();
        }
        
        [ContextMenu("Export Endings to CSV")]
        public void ExportEndings()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Endings.csv");
            file.WriteLine("id;description_RU;description_EN");
            foreach (var ending in endings)
            {
                file.WriteLine(ending.name+";"+ending.text);
            }
            file.Close();
        }
    }
}