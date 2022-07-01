using CauldronCodebase;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Editor
{
    [CustomEditor(typeof(EncounterDeckAutoFilled))]
    public class EncounterDeckAutoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load cards"))
            {
                LoadCards();    
            }
            base.OnInspectorGUI();
        }

        void LoadCards()
        {
            var deck = target as EncounterDeckAutoFilled;
            var dict = new SortedList<int, List<Encounter>>();
            Encounter[] cards = ScriptableObjectHelper.LoadAllAssets<Encounter>();
            foreach (var card in cards)
            {
                if (card.addToDeckOnDay < 0)
                {
                    continue;
                }
                if (dict.TryGetValue(card.addToDeckOnDay, out var list))
                {
                    list.Add(card);
                }
                else
                {
                    dict.Add(card.addToDeckOnDay, new List<Encounter>(3) {card});
                }
            }

            var listPool = new List<EncounterDeckAutoFilled.CardPoolPerDay>();
            foreach (var keyValuePair in dict)
            {
                listPool.Add(new EncounterDeckAutoFilled.CardPoolPerDay(
                    keyValuePair.Key, keyValuePair.Value.ToArray()));
            }
            
            deck.cardPoolsByDay = listPool.ToArray();
            EditorUtility.SetDirty(deck);
        }
    }
}