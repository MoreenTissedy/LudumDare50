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

        private Visitor currentVisitor;
        private int attemptsLeft;
        private float attemptsTimerStep;

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
            //TODO check if visitor exists
            attemptsLeft--;
            visitorText.ReduceTimer(attemptsTimerStep);
            if (attemptsLeft <= 0)
            {
                Debug.LogError("visitor left while waiting");
                Exit();
                VisitorLeft?.Invoke();
            }
        }
        
        public void Enter(Encounter card)
        {
            ShowText(card);
            //Debug.Log("enter card"+card.name);
            Villager villager = card.actualVillager;
            attemptsLeft = villager.patience;
            attemptsTimerStep = 1f / attemptsLeft;
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

        public void EnterDefault()
        {
            visitors[0].Enter();
            currentVisitor = visitors[0];
        }

        public void Exit()
        {
            Debug.Log("exit visitor "+currentVisitor.name);
            HideText();
            currentVisitor.Exit();
        }
    }
}