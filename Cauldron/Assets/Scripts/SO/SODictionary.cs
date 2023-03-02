using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CauldronCodebase
{

    [CreateAssetMenu]
    public class SODictionary : ScriptableObject
    {
        public Dictionary<string, ScriptableObjectWithId> AllScriptableObjects = new Dictionary<string, ScriptableObjectWithId>();

        public List<string> AllSOKeys = new List<string>();
        public List<ScriptableObjectWithId> AllSOValues = new List<ScriptableObjectWithId>();
        

        public void LoadDictionary()
        {
            for (int i = 0; i < AllSOKeys.Count; i++)
            {
                AllScriptableObjects.Add(AllSOKeys[i], AllSOValues[i]);
            }
        }
    }
}