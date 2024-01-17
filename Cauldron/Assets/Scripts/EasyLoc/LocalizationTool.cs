using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Reflection;
using CauldronCodebase;
using Zenject;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace EasyLoc
{
    public class LocalizationTool
    {
        private Language loadedLanguage;
        public event Action<Language> OnLanguageChanged;

        [Inject]
        private void Startup()
        {
            LoadLanguage(GetSavedLanguage());
        }

        public Language GetSavedLanguage()
        {
            var language = Language.EN;
            if (PlayerPrefs.HasKey(PrefKeys.LanguageKey) &&
                PlayerPrefs.GetString(PrefKeys.LanguageKey) == Language.RU.ToString())
            {
                language = Language.RU;
            }

            return language;
        }

        public void LoadLanguage(Language language)
        {
            if (loadedLanguage == language)
            {
                return;
            }
            ImportScriptableObjects(language);
            loadedLanguage = language;
            OnLanguageChanged?.Invoke(language);
        }

        private void ImportScriptableObjects(Language language)
        {
            Debug.Log("[Loc] translating SOs to "+language+"...");
            var units = Resources.FindObjectsOfTypeAll<LocalizableSO>();
            foreach (LocalizableSO unit in units)
            {
                try
                {
                    if (!unit.Localize(language))
                    {
                        Debug.LogWarning(unit.name + " not found in " + unit.localizationCSV.name);
                    }
                    #if UNITY_EDITOR
                    else
                    {
                        if (!Application.isPlaying) EditorUtility.SetDirty(unit);
                    }
                    #endif
                }
                catch (Exception e)
                {
                    Debug.LogError("failed to localize " + unit.name + ": " + e.Message);
                    continue;
                }
            }

#if UNITY_EDITOR
            if (!Application.isPlaying) AssetDatabase.SaveAssets();
#endif
        }

        
        //TODO: collectUI directly in scenes
#if UNITY_EDITOR
        [ContextMenu("Collect UI")]
        void CollectFieldsWithAttribute()
        {
            //load all scenes
            List<string> scenePaths = EditorBuildSettings.scenes.Select(scene => scene.path).ToList();
            foreach (string scenePath in scenePaths)
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            MonoBehaviour[] sceneActive = GameObject.FindObjectsOfType<MonoBehaviour>(true);
            HashSet<string> uniqueKeys = new HashSet<string>();

            var file = File.CreateText(Application.dataPath + "/Localize/UI.csv");
            file.WriteLine("class;id;string_RU");
            
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
                        string id;
                        if (mono is ILocTextTool textTool)
                        {
                            id = textTool.GetId();
                        }
                        else
                        {
                            id = objectFields[i].Name;
                        }
                        string key = $"{monoType.ToString()};{id}";
                        if (uniqueKeys.Contains(key))
                        {
                            Debug.LogWarning("duplicate key "+key);
                            continue;
                        }
                        uniqueKeys.Add(key);
                        file.WriteLine($"{key};{objectFields[i].GetValue(mono)}");
                        Debug.Log($"{key};{objectFields[i].GetValue(mono)}");
                    }
                }
            }
            file.Close();
            Debug.Log("ui collected!");
        }
#endif

    }
}