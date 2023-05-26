using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public class SODictionaryLoader
    {
        static SODictionaryLoader()
        {
            EditorApplication.projectChanged += LoadDictionary;
        }

        static void LoadDictionary()
        {
            var dictionary = ScriptableObjectHelper.LoadSingleAsset<SODictionary>();

            ScriptableObjectWithId[] sObjects = ScriptableObjectHelper.LoadAllAssets<ScriptableObjectWithId>();

            dictionary.AllSOKeys.Clear();
            dictionary.AllSOValues.Clear();
            foreach (var obj in sObjects)
            {
                
                obj.Id = obj.name;
                EditorUtility.SetDirty(obj);
                
                if (dictionary.AllSOKeys.Contains(obj.name))
                {
                    Debug.LogWarning($"Detect double: {obj.name}");
                }
                
                dictionary.AllSOKeys.Add(obj.Id);
                dictionary.AllSOValues.Add(obj);
            }

            EditorUtility.SetDirty(dictionary);
        }
    }
    
}