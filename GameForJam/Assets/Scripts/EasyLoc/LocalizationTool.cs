using System;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
                if (!unit.Localize(selectLanguage))
                    Debug.LogWarning(unit.name+" not found in "+unit.localizationCSV.name);
            }
            ImportUI();
        }

        [ContextMenu("Collect UI")]
        void CollectFieldsWithAttribute()
        {
            //only current scene
            MonoBehaviour[] sceneActive = GameObject.FindObjectsOfType<MonoBehaviour>();

            var file = File.CreateText(Application.dataPath + "/Localize/UI.csv");
            file.WriteLine("class;id;string_RU");
            
            foreach (MonoBehaviour mono in sceneActive)
            {
                Type monoType = mono.GetType();

                // Retreive the fields from the mono instance
                FieldInfo[] objectFields = monoType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                // search all fields and find the attribute [Localize]
                for (int i = 0; i < objectFields.Length; i++)
                {
                    // if we detect any attribute collect the data into the CSV.
                    if (Attribute.GetCustomAttribute(objectFields[i],
                        typeof(LocalizeAttribute)) is LocalizeAttribute attribute)
                    {
                        string id;
                        if (mono is EasyLocTextTool textTool)
                        {
                            id = textTool.id;
                        }
                        else
                        {
                            id = objectFields[i].Name;
                        }
                        file.WriteLine("{0};{1};{2}", 
                            monoType.ToString(), 
                            id, 
                            objectFields[i].GetValue(mono)
                            );
                    }
                }
            }
            file.Close();
        }

        void ImportUI()
        {
            Debug.Log("UI import");
            //fetch data from csv into a dictionary
            Dictionary<string, string> locData = new Dictionary<string, string>();
            string[] lines = UI.text.Split('\n');
            string[] headers = lines[0].Split(';');
            int langIndex = -1;
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+selectLanguage.ToString()))
                {
                    langIndex = i;
                    break;
                }
            }

            if (langIndex < 0)
                return;
            
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                string[] data = line.Split(';');
                if (langIndex >= data.Length)
                {
                    continue;
                }

                data[langIndex] = data[langIndex].Replace('>', '\n');
                locData.Add(data[1],data[langIndex]);
            }

            //search all Monobeh scripts, only current scene
            MonoBehaviour[] sceneActive = GameObject.FindObjectsOfType<MonoBehaviour>();
            
            foreach (MonoBehaviour mono in sceneActive)
            {
                Type monoType = mono.GetType();

                // Retreive the fields from the monobeh
                FieldInfo[] objectFields = monoType.GetFields(BindingFlags.Instance | BindingFlags.Public);

                // search all fields and find the attribute [Localize]
                for (int i = 0; i < objectFields.Length; i++)
                {
                    // if we detect the attribute, try to find the respective localization data by id.
                    if (Attribute.GetCustomAttribute(objectFields[i],
                        typeof(LocalizeAttribute)) is LocalizeAttribute)
                    {
                        bool hasData = false;
                        //for UI.Text components we have special TextTool script
                        //that allows us to specify the text ID (but doesn't ensure its uniqueness for now)
                        if (mono is EasyLocTextTool textTool)
                        {
                            if (locData.TryGetValue(textTool.id, out string textValue))
                            {
                                hasData = true;
                                textTool.SetText(textValue);
                                Debug.Log(textTool.id+" "+textValue);
                            }
                        }
                        //for the custom scripts their respective class and field act as ID
                        else
                        {
                            if (locData.TryGetValue(objectFields[i].Name, out string textValue))
                            {
                                hasData = true;
                                objectFields[i].SetValue(mono, textValue);
                                Debug.Log(objectFields[i].Name+" "+textValue);
                            }
                        }
                        //changes to prefab instances in editor are not recorded automatically,
                        //so the values are reverted to prefab defaults at the very first possibility.
                        //to change this behaviour we need this:
                        if (hasData && PrefabUtility.IsPartOfPrefabInstance(mono))
                        {
                            PrefabUtility.RecordPrefabInstancePropertyModifications(mono);
                            Debug.Log("record prefab");
                        }
                    }
                }
            }
            //signal that scene has changed
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
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