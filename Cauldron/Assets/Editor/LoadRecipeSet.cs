using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RecipeProvider))]
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
            RecipeProvider provider = target as RecipeProvider;
            if (provider is null)
                return;
            provider.allRecipes = ScriptableObjectHelper.LoadAllAssets<Recipe>();
        }
    }
}