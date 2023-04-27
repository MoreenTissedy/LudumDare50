using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using NUnit.Framework;
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
            List<ConditionalEvent> conditionalDeck = new List<ConditionalEvent>(data.Count);
            List<RandomNightEvent> randomDeck = new List<RandomNightEvent>(data.Count);
            //randomize
            int dataCount = data.Count;
            for (int i = 0; i < dataCount; i++)
            {
                int dice = Random.Range(0, data.Count);
                if (data[dice] as RandomNightEvent)
                {
                    randomDeck.Add((RandomNightEvent) data[dice]);
                }
                else
                {
                    conditionalDeck.Add(data[dice]);
                }
                data.RemoveAt(dice);
            }
            provider.conditionalEvents = conditionalDeck;
            provider.randomEvents = randomDeck;
            EditorUtility.SetDirty(provider);
        }
    }
}