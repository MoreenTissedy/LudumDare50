using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{

    [CreateAssetMenu]
    public class SODictionary : ScriptableObject
    {
        public bool AutoUpdateDictionary;
        
        [Space(15)]
        public List<string> AllSOKeys = new List<string>();
        public List<ScriptableObjectInDictionary> AllSOValues = new List<ScriptableObjectInDictionary>();
        
        public readonly Dictionary<string, ScriptableObjectInDictionary> AllScriptableObjects = new Dictionary<string, ScriptableObjectInDictionary>();


        public void LoadDictionary()
        {
            for (int i = 0; i < AllSOKeys.Count; i++)
            {
                if (AllScriptableObjects.ContainsKey(AllSOKeys[i]))
                {
                    Debug.LogWarning(i + " "+ AllSOValues[i]);
                    continue;
                }
                AllScriptableObjects.Add(AllSOKeys[i], AllSOValues[i]);
            }
        }
    }
}