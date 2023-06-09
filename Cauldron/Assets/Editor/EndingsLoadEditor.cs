using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EndingsProvider))]
    public class EndingsLoadEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load cards"))
            {
                Load();    
            }
            base.OnInspectorGUI();
        }

        private void Load()
        {
            var provider = target as EndingsProvider;
            provider.endings = ScriptableObjectHelper.LoadAllAssets<Ending>();
            EditorUtility.SetDirty(provider);
        }
    }
}