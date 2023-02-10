using System;
using Save;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class VisitorManager : MonoBehaviour, IDataPersistence
    {
        public GameObject witchCat;
        public Villager[] villagers;
        public Visitor[] visitors;
        
        public VisitorTextBox visitorText;
        public VisitorTimer visitorTimer;

        [SerializeField] private Visitor currentVisitor;
        private int attemptsLeft;
        
        private Cauldron cauldron;
        public event Action VisitorLeft;

        private bool ignoreSavedAttempts = false;
        private GameData gameData;

        [Inject]
        private void Init(Cauldron cauldron, DataPersistenceManager dataPersistenceManager)
        {
            this.cauldron = cauldron;
            dataPersistenceManager.AddToDataPersistenceObjList(this);
        }
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

            switch (ignoreSavedAttempts)
            {
                case true:
                    attemptsLeft = villager.patience;
                    break;
                case false:
                    attemptsLeft = gameData.AttemptsLeft;
                    ignoreSavedAttempts = true;
                    break;
            }
            
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
            if(currentVisitor == null) return;
            
            HideText();
            currentVisitor.Exit();
            currentVisitor = null;
        }

        public void LoadData(GameData data, bool newGame)
        {
            ignoreSavedAttempts = newGame;
            gameData = data;
        }

        public void SaveData(ref GameData data)
        {
            if(data == null) return;
            
            data.AttemptsLeft = attemptsLeft;
        }
    }
}