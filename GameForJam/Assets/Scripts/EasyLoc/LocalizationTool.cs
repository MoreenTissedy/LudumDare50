using System;
using System.IO;
using DefaultNamespace;
using UnityEngine;

namespace EasyLoc
{
    public class LocalizationTool : MonoBehaviour
    {
        public TextAsset UI;

        [Header ("Load Language with right click!")]
        [ContextMenuItem("Load language", "LoadCurrentLanguage")]
        public Language selectLanguage;

        public Language loadedLanguage;

        void LoadCurrentLanguage()
        {
            loadedLanguage = selectLanguage;
            foreach (LocalizableSO unit in Resources.FindObjectsOfTypeAll<LocalizableSO>())
            {
                unit.Localize(selectLanguage);
            }
        }

        [ContextMenu("Export Cards Tool")]
        //temp helper
        void ExportCards()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Cards.csv");
            file.WriteLine("id;description_RU;description_EN");
            foreach (Encounter unit in Resources.FindObjectsOfTypeAll<Encounter>())
            {
                file.WriteLine(unit.name+";"+unit.text);
            }
            file.Close();
        }

    }
}