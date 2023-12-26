using System.Collections.Generic;
using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    //[CustomEditor(typeof(RecipeProvider))]
    public class LoadRecipeSet : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load recipes"))
            {
                LoadRecipes();
            }
            DrawDefaultInspector();
        }

        void LoadRecipes()
        {
            RecipeProvider provider = target as RecipeProvider;
            if (provider is null)
                return;
            
            List<Recipe> data = ScriptableObjectHelper.LoadAllAssetsList<Recipe>();
            
            int dataCount = data.Count;
            List<Recipe> deck = new List<Recipe>(dataCount);

            for (int i = 0; i < dataCount; i++)
            {
                int dice = Random.Range(0, data.Count);
                deck.Add(data[dice]);
                data.RemoveAt(dice);
            }

            provider.allRecipes = deck.ToArray();
            EditorUtility.SetDirty(provider);
        }
    }
}