using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    [CustomEditor(typeof(SODictionary))]
    public class SODictionaryLoader : UnityEditor.Editor
    {
        private static SODictionary targetDictionary;
        
        static SODictionaryLoader()
        {
            EditorApplication.projectChanged += AutoLoadDictionary;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Update SODictionary", GUILayout.Height(40)))
            {
                var dictionary = target as SODictionary;
                
                LoadInternal(dictionary);
            }

            base.OnInspectorGUI();
        }

        static void LoadInternal(SODictionary dictionary)
        {
            var sObjects = ScriptableObjectHelper.LoadAllAssets<ScriptableObjectInDictionary>();
            
            dictionary.AllSOKeys.Clear();
            dictionary.AllSOValues.Clear();
            foreach (var obj in sObjects)
            {
                if (dictionary.AllSOKeys.Contains(obj.name))
                {
                    Debug.LogError($"Detect double: {obj.name}");
                }
                
                dictionary.AllSOKeys.Add(obj.name);
                dictionary.AllSOValues.Add(obj);
            }
            
            EditorUtility.SetDirty(dictionary);
        }

        static void AutoLoadDictionary()
        {
            if(targetDictionary == null) 
                targetDictionary = ScriptableObjectHelper.LoadSingleAsset<SODictionary>();

            if(targetDictionary.AutoUpdateDictionary == false) return;
            
            LoadInternal(targetDictionary);
        }
    }
    
}