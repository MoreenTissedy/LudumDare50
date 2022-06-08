using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RecipeSet))]
    public class LoadRecipeSet : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load recipes"))
            {
                LoadRecipes();
            }
            base.OnInspectorGUI();
        }

        void LoadRecipes()
        {
            RecipeSet set = target as RecipeSet;
            if (set is null)
                return;
            
            List<Recipe> recipes = new List<Recipe>();
            string[] guids = AssetDatabase.FindAssets("t: Recipe");
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                Recipe asset = AssetDatabase.LoadAssetAtPath<Recipe>( assetPath );
                if( asset != null )
                {
                    recipes.Add(asset);
                }
            }

            set.allRecipes = recipes.ToArray();
        }
    }
}