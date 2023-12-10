using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace EasyLoc
{
    public abstract class BaseLocalizer: MonoBehaviour
    {
        public TextAsset UI;
        public List<MonoBehaviour> localizationUnits;
        
        [Inject] protected LocalizationTool localizationTool;
        
        private void Start()
        {
            ImportUI(localizationTool.GetSavedLanguage());
            localizationTool.OnLanguageChanged += ImportUI;
        }

        private void OnDestroy()
        {
            localizationTool.OnLanguageChanged -= ImportUI;
        }

        protected abstract MonoBehaviour[] GetLocalizationObjects();
        
        #if UNITY_EDITOR
        [Button("Collect UI")] [UsedImplicitly]
        void CollectFieldsWithAttribute()
        {
            localizationUnits = new List<MonoBehaviour>(10);
            MonoBehaviour[] sceneActive = GetLocalizationObjects();
            
            foreach (MonoBehaviour mono in sceneActive)
            {
                Type monoType = mono.GetType();

                // Retrieve the fields from the mono instance
                FieldInfo[] objectFields = monoType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // search all fields and find the attribute [Localize]
                for (int i = 0; i < objectFields.Length; i++)
                {
                    // if we detect any attribute collect the data into the CSV.
                    if (Attribute.GetCustomAttribute(objectFields[i],
                        typeof(LocalizeAttribute)) is LocalizeAttribute attribute)
                    {
                        if (localizationUnits.Contains(mono))
                        {
                            continue;
                        }
                        localizationUnits.Add(mono);
                    }
                }
            }
        }
        #endif
        
        void ImportUI(Language language)
        {
            //fetch data from csv into a dictionary
            Dictionary<string, string> locData = new Dictionary<string, string>();
            string[] lines = UI.text.Split('\n');
            string[] headers = lines[0].Split(';');
            int langIndex = -1;
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+language.ToString()))
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

            foreach (MonoBehaviour mono in localizationUnits)
            {
                if (mono is null)
                {
                    continue;
                }
                Type monoType = mono.GetType();

                // Retreive the fields from the monobeh
                FieldInfo[] objectFields = monoType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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
                        if (mono is ILocTextTool textTool)
                        {
                            if (locData.TryGetValue(textTool.GetId(), out string textValue))
                            {
                                hasData = true;
                                textTool.SetText(textValue.Replace(">", "\n"));
                            }
                        }
                        //for the custom scripts their respective class and field act as ID
                        else
                        {
                            if (locData.TryGetValue(objectFields[i].Name, out string textValue))
                            {
                                hasData = true;
                                objectFields[i].SetValue(mono, textValue.Replace(">", "\n"));
                            }
                        }
                        //changes to prefab instances in editor are not recorded automatically,
                        //so the values are reverted to prefab defaults at the very first possibility.
                        //to change this behaviour we need this:
                        #if UNITY_EDITOR
                        if (Application.isPlaying)
                        {
                            continue;
                        }
                        if (hasData && UnityEditor.PrefabUtility.IsPartOfPrefabInstance(mono))
                        {
                            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(mono);
                            Debug.Log("record prefab");
                        }
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                        #endif
                    }
                }
            }
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }
            //signal that scene has changed
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
        }
    }
}