using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SODictionary))]
    public class SODictionaryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load Dictionary"))
            {
                LoadDictionary();    
            }
            base.OnInspectorGUI();
        }
        
        void LoadDictionary()
        {
            var dictionary = target as SODictionary;

            ScriptableObjectWithId[] sObjects = ScriptableObjectHelper.LoadAllAssets<ScriptableObjectWithId>();

            dictionary.AllSOKeys.Clear();
            dictionary.AllSOValues.Clear();
            foreach (var obj in sObjects)
            {
                if (string.IsNullOrEmpty(obj.Id))
                {
                    obj.Id = GUID.Generate().ToString();
                    EditorUtility.SetDirty(obj);
                }

                dictionary.AllSOKeys.Add(obj.Id);
                dictionary.AllSOValues.Add(obj);
                //Debug.Log(dictionary.AllEncounters.Count);
            }

            EditorUtility.SetDirty(dictionary);
        }
    }
}