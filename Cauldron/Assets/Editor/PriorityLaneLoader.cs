using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PriorityLaneProvider))]
    public class PriorityLaneLoader : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load cards"))
            {
                LoadCards();    
            }
            base.OnInspectorGUI();
        }

        private void LoadCards()
        {
            var provider = target as PriorityLaneProvider;
            var list = new List<Encounter>(10);
            Encounter[] cards = ScriptableObjectHelper.LoadAllAssets<Encounter>();
            foreach (var card in cards)
            {
                if (card.requiredStoryTag.Split(',').Any((x) => x.Trim() == EndingsProvider.LOW_FAME ||
                                                                x.Trim() == EndingsProvider.HIGH_FAME
                                                                || x.Trim() == EndingsProvider.LOW_FEAR ||
                                                                x.Trim() == EndingsProvider.HIGH_FEAR))
                {
                    card.addToDeckOnDay = -1;
                    EditorUtility.SetDirty(card);
                    list.Add(card);
                }
            }
            provider.priorityCards = list.ToArray();
            EditorUtility.SetDirty(provider);
        }
    }
}