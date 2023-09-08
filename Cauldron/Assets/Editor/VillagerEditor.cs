using CauldronCodebase;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Villager))]
    public class VillagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Villager villager = target as Villager;
            if (villager.visitorPrefab)
            {
                GUILayout.Box(new GUIContent(AssetPreview.GetAssetPreview(villager.visitorPrefab.GetComponent<SkeletonAnimation>().skeletonDataAsset)));
            }
        }
    }
}