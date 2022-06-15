using System;
using UnityEngine;

namespace CauldronCodebase
{
    public class VisitorManager : MonoBehaviour
    {
        public Villager[] villagers;
        public Visitor[] visitors;
        
        public VisitorTextBox visitorText;

        private Visitor currentVisitor;

        private void Awake()
        {
            HideText();
        }

        public void ShowText(Encounter card)
        {
            visitorText.Display(card);
        }

        public void HideText()
        {
            visitorText.Hide();
        }
        
        public void Enter(Encounter card)
        {
            ShowText(card);
            Villager villager = card.actualVillager;
            for (int i = 0; i < villagers.Length; i++)
            {
                if (villagers[i] == villager)
                {
                    visitors[i].Enter();
                    currentVisitor = visitors[i];
                    break;
                }
            }
        }

        public void EnterDefault()
        {
            visitors[0].Enter();
            currentVisitor = visitors[0];
        }

        public void Exit()
        {
            HideText();
            currentVisitor.Exit();
        }
    }
}