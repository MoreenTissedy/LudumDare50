using CauldronCodebase;
using UnityEditor;
using UnityEngine;
namespace Editor
{
    [CustomEditor(typeof(SkinsProvider))]
    public class SkinsLoadEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load skins"))
            {
                Load();    
            }
            base.OnInspectorGUI();
        }

        private void Load()
        {
            var provider = target as SkinsProvider;
            provider.skins = ScriptableObjectHelper.LoadAllAssets<SkinSO>();
            EditorUtility.SetDirty(provider);
        }
    }
}
