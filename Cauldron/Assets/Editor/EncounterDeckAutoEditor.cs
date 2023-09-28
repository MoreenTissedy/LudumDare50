using CauldronCodebase;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Editor
{
    [CustomEditor(typeof(EncounterDeck))]
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
            var deck = target as EncounterDeck;
            var dict = new SortedList<int, List<Encounter>>();
            Encounter[] cards = ScriptableObjectHelper.LoadAllAssets<Encounter>();
            foreach (var card in cards)
            {
                if (card.addToDeckOnRound < 0)
                {
                    continue;
                }
                if (dict.TryGetValue(card.addToDeckOnRound, out var list))
                {
                    list.Add(card);
                }
                else
                {
                    dict.Add(card.addToDeckOnRound, new List<Encounter>(3) {card});
                }
            }

            var listPool = new List<CardPoolByRound>();
            foreach (var keyValuePair in dict)
            {
                listPool.Add(new CardPoolByRound(
                    keyValuePair.Key, keyValuePair.Value.ToArray()));
            }
            
            deck.cardPoolsByRound = listPool.ToArray();
            EditorUtility.SetDirty(deck);
        }
    }
}