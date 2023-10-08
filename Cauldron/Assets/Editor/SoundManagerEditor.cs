using System;
using System.Collections.Generic;
using Editor;
using UnityEditor;
using UnityEditor.Experimental;
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
                EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i), new GUIContent(Enum.GetName(typeof(Sounds), i)));
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(new GUIContent("Music"));
            var music = serializedObject.FindProperty("musics");
            music.arraySize = Enum.GetNames(typeof(Music)).Length;
            for (int i = 0; i < music.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(music.GetArrayElementAtIndex(i), new GUIContent(Enum.GetName(typeof(Music), i)));
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Separator();
            var visitors = serializedObject.FindProperty("defaultVisitorSounds");
            EditorGUILayout.PropertyField(visitors);
            
            EditorGUILayout.Separator();
            var cat = serializedObject.FindProperty("catSounds");
            EditorGUILayout.PropertyField(cat);
            
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(new GUIContent("Potion effects"));
            var effects = serializedObject.FindProperty("potionEffects");
            var potions = GetListOfPotions();
            effects.arraySize = potions.Length;
            for (int i = 0; i < potions.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(effects.GetArrayElementAtIndex(i), new GUIContent(Enum.GetName(typeof(Potions), potions[i])));
                EditorGUILayout.EndHorizontal();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private int[] GetListOfPotions()
        {
            RecipeProvider provider = ScriptableObjectHelper.LoadSingleAsset<RecipeProvider>();
            List<int> list = new List<int>(10);
            for (var index = 0; index < 20; index++)
            {
                var recipe = (Potions)index;
                if (provider.GetRecipeForPotion(recipe).magical)
                {
                    list.Add(index);
                }
            }
            
            list.Add(99);
            list.Add(105);
            return list.ToArray();
        }
    }
}