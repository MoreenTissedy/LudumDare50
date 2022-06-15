using System.IO;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Endings Provider", menuName = "Endings Provider", order = 10)]
    public class EndingsProvider : ScriptableObject
    {
        [Tooltip("High money, high fame, high fear, low fame, low fear")]
        public Ending[] endings;
        public int threshold = 70;
        public Encounter[] highFameCards, highFearCards;
        
         
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

            //high status cards
            if (gm.GameState.Fame > threshold)
            {
                gm.CardDeck.AddToDeck(Encounter.GetRandom(highFameCards), true);
            }

            if (gm.GameState.Fear > threshold)
            {
                gm.CardDeck.AddToDeck(Encounter.GetRandom(highFearCards), true);
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