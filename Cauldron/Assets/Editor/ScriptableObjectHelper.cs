using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ScriptableObjectHelper
    {
        public static T[] LoadAllAssets<T>() where T:ScriptableObject
        {
            return LoadAllAssetsList<T>().ToArray();
        }

        public static List<T> LoadAllAssetsList<T>() where T : ScriptableObject
        {
            List<T> data = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t: {typeof(T)}");
            for( int i = 0; i < guids.Length; i++ )
            {
                string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
                T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
                if( asset != null )
                {
                    data.Add(asset);
                }
            }

            return data;
        }
    }
}