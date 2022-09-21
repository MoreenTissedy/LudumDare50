using System.IO;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Endings Provider", menuName = "Endings Provider", order = 10)]
    public class EndingsProvider : ScriptableObject
    {
        [Tooltip("High money, high fame, high fear, low fame, low fear")]
        public Ending[] endings;
        //TODO save this and init 
        public bool[] unlocked;
        public int threshold = 70;
        public int lowThreshold = 20;

        public void Init()
        {
            unlocked = new bool[endings.Length];
        }
        public int GetIndexOf(Ending ending)
        {
            for (var i = 0; i < endings.Length; i++)
            {
                if (ending == endings[i])
                    return i;
            }

            return -1;
        }
         
        public void StatusChecks(GameManager gm)
        {
            //endings
            //[Tooltip("High money, high fame, high fear, low fame, low fear")]
            if (gm.GameState.Fame >= gm.Settings.gameplay.statusBarsMax)
            {
                gm.EndGame(endings[1]);
                return;
            }

            if (gm.GameState.Fear >= gm.Settings.gameplay.statusBarsMax)
            {
                gm.EndGame(endings[2]);
                return;
            }

            if (gm.GameState.Money >= gm.Settings.gameplay.statusBarsMax)
            {
                gm.EndGame(endings[0]);
                return;

            }

            if (gm.GameState.Fame <= 0)
            {
                gm.EndGame(endings[3]);
                return;
            }

            if (gm.GameState.Fear <= 0)
            {
                gm.EndGame(endings[4]);
                return;
            }

            AddHighLowTag(gm.GameState.Fear, "high fear");
            AddHighLowTag(gm.GameState.Fear, "low fear", false);
            AddHighLowTag(gm.GameState.Fame, "high fame");
            AddHighLowTag(gm.GameState.Fame, "low fame", false);

            void AddHighLowTag(int status, string tag, bool checkHigh = true)
            {
                bool thresholdReached = checkHigh ? status > threshold : status < lowThreshold;
                if (thresholdReached)
                {
                    gm.GameState.AddTag(tag);
                }
                else
                {
                    gm.GameState.storyTags.Remove(tag);
                }
            }
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