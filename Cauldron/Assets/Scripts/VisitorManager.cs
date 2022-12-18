using System;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class VisitorManager : MonoBehaviour
    {
        public GameObject witchCat;
        public Villager[] villagers;
        public Visitor[] visitors;
        
        public VisitorTextBox visitorText;
        public VisitorTimer visitorTimer;

        private Visitor currentVisitor;
        private int attemptsLeft;

        [Inject]
        private Cauldron cauldron;
        public event Action VisitorLeft;

        private void Awake()
        {
            HideText();
            cauldron.PotionDeclined += Wait;
        }

        private void ShowText(Encounter card)
        {
            visitorText.Display(card);
        }

        private void HideText()
        {
            visitorText.Hide();
        }

        private void Wait()
        {
            if (currentVisitor is null)
            {
                return;
            }
            attemptsLeft--;
            visitorTimer.ReduceTimer();
            if (attemptsLeft <= 0)
            {
                Exit();
                VisitorLeft?.Invoke();
            }
        }
        
        public void Enter(Encounter card)
        {
            ShowText(card);
            Villager villager = card.actualVillager;
            attemptsLeft = villager.patience;
            visitorTimer.ResetTimer(attemptsLeft);
            for (int i = 0; i < villagers.Length; i++)
            {
                if (villagers[i] == villager)
                {
                    visitors[i].Enter();
                    currentVisitor = visitors[i];
                    break;
                }
            }
            
            //if cat - disable cat, else - enable cat
            witchCat.SetActive(villager.name != "Cat");
        }

        public void Exit()
        {
            HideText();
            currentVisitor.Exit();
            currentVisitor = null;
        }
    }
}