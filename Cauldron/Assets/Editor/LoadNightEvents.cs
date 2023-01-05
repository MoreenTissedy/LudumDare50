using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(NightEventProvider))]
    public class LoadNightEvents : UnityEditor.Editor
    {
        private NightEventProvider provider;
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load events"))
            {
                if (!FindTargetScript()) return;
                LoadConditionalEvents();
            }
            base.OnInspectorGUI();
        }

        private bool FindTargetScript()
        {
            provider = target as NightEventProvider;
            if (provider is null)
            {
                Debug.LogError("Something is wrong with Event Provider");
                return false;
            }
            return true;
        }

        private void LoadConditionalEvents()
        {
            List<ConditionalEvent> data = ScriptableObjectHelper.LoadAllAssetsList<ConditionalEvent>();
            List<ConditionalEvent> deck = new List<ConditionalEvent>(data.Count);
            //randomize
            int dataCount = data.Count;
            List<ConditionalEvent> defaultEvents = new List<ConditionalEvent>(3);
            for (int i = 0; i < dataCount; i++)
            {
                int dice = Random.Range(0, data.Count);
                if (data[dice].defaultEvent)
                {
                    defaultEvents.Add(data[dice]);
                }
                else
                {
                    deck.Add(data[dice]);
                }
                data.RemoveAt(dice);
            }
            deck.AddRange(defaultEvents);
            provider.conditionalEvents = deck;
            EditorUtility.SetDirty(provider);
        }
    }
}