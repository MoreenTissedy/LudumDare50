using CauldronCodebase;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(Villager))]
    public class VillagerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.PropertyField(position, property);
            DrawPreview(property.objectReferenceValue as Villager);
        }

        private static void DrawPreview(Villager villager)
        {
            if (villager && villager.visitorPrefab)
            {
                GUILayout.Box(new GUIContent(AssetPreview.GetAssetPreview(villager.visitorPrefab
                    .GetComponent<SkeletonAnimation>().skeletonDataAsset)));
            }
        }
    }
}