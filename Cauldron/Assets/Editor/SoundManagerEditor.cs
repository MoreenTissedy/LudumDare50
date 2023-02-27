using System;
using UnityEditor;
using UnityEngine;

namespace CauldronCodebase.Editor
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var values = serializedObject.FindProperty("sounds");
            values.arraySize = Enum.GetNames(typeof(Sounds)).Length;
            for (int i = 0; i < values.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Enum.GetName(typeof(Sounds), i), GUILayout.Width(100));
                EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}