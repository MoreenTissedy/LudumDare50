using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using CauldronCodebase;
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace EasyLoc
{
    public class LocalizationTool : MonoBehaviour
    {
        public TextAsset UI;

        [Header ("Load Language with right click!")]
        [ContextMenuItem("Load language", "LoadCurrentLanguage")]
        [SerializeField] private Language selectLanguage;

        public Language loadedLanguage;

        #if UNITY_EDITOR
        
        [UsedImplicitly]
        void LoadCurrentLanguage()
        {
            //TODO: make a normal interface
            //Load all scenes
            LoadLanguage(selectLanguage);
            loadedLanguage = selectLanguage;
            //unload scenes
        }

        public void LoadSavedLanguage()
        {
            var language = Language.EN;
            if (PlayerPrefs.HasKey(PrefKeys.LanguageKey) &&
                PlayerPrefs.GetString(PrefKeys.LanguageKey) == Language.RU.ToString())
            {
                language = Language.RU;
            }
            LoadLanguage(language);
        }

        public void LoadLanguage(Language language)
        {
            Debug.LogWarning("changing language to " + language);
            var units = GetUnits();
            Debug.Log("Found SO to localize: " + units.Length);
            foreach (LocalizableSO unit in units)
            {
                try
                {
                    if (!unit.Localize(language))
                    {
                        Debug.LogWarning(unit.name + " not found in " + unit.localizationCSV.name);
                    }
                    else
                    {
                        if (!Application.isPlaying) EditorUtility.SetDirty(unit);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("failed to localize "+unit.name+": "+e.Message);
                    continue;
                }
            }
            if (!Application.isPlaying) AssetDatabase.SaveAssets();
            ImportUI(language);
            loadedLanguage = language;
        }

        LocalizableSO[] GetUnits()
        {
            var guids = AssetDatabase.FindAssets("t: LocalizableSO");
            List<LocalizableSO> list = new List<LocalizableSO>(guids.Length);
            foreach (var guid in guids)
            {
                list.Add(AssetDatabase.LoadAssetAtPath<LocalizableSO>(AssetDatabase.GUIDToAssetPath(guid)));
            }
            return list.ToArray();
        }
        #endif

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

        void ImportUI(Language language)
        {
            Debug.Log("UI import");
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

            //search all Monobeh scripts, only current scene
            MonoBehaviour[] sceneActive = GameObject.FindObjectsOfType<MonoBehaviour>(true);
            
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
                        if (mono is ILocTextTool textTool)
                        {
                            if (locData.TryGetValue(textTool.GetId(), out string textValue))
                            {
                                hasData = true;
                                textTool.SetText(textValue);
                                Debug.Log(textTool.GetId()+" "+textValue);
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