using System;
using Cysharp.Threading.Tasks;
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

        private int currentVisitorIndex = -1;
        private int attemptsLeft;
        
        private Cauldron cauldron;
        public event Action VisitorLeft;

        private bool ignoreSavedAttempts = false;
        private GameData gameData;
        private SoundManager soundManager;
        private DataPersistenceManager dataPersistenceManager;

        [Inject]
        private void Init(Cauldron cauldron, DataPersistenceManager dataPersistenceManager, SoundManager soundManager)
        {
            this.soundManager = soundManager;
            this.cauldron = cauldron;
            dataPersistenceManager.AddToDataPersistenceObjList(this);
            this.dataPersistenceManager = dataPersistenceManager;
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
            if (currentVisitorIndex < 0)
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
            else
            {
                dataPersistenceManager.SaveGame();
            }
        }
        
        public async void Enter(Encounter card)
        {
            Villager villager = card.actualVillager;

            //TODO: refactor
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
            
            visitorTimer.ResetTimer(villager.patience);
            if (attemptsLeft != villager.patience)
            {
                for (int i = 0; i < villager.patience - attemptsLeft; i++)
                {
                    visitorTimer.ReduceTimer();
                }
            }
            
            for (int i = 0; i < villagers.Length; i++)
            {
                if (villagers[i] == villager)
                {
                    soundManager.PlayVisitor(villager.sounds, VisitorSound.Door);
                    await UniTask.Delay(300);
                    soundManager.PlayVisitor(villager.sounds, VisitorSound.Enter);
                    visitors[i].Enter();
                    soundManager.PlayVisitor(villager.sounds, VisitorSound.Speech);
                    currentVisitorIndex = i;
                    break;
                }
            }

            await UniTask.Delay(150);
            ShowText(card);
            
            //if cat - disable cat, else - enable cat
            witchCat.SetActive(villager.name != "Cat");
        }

        public void Exit()
        {
            if(currentVisitorIndex < 0) return;
            
            HideText();
            visitors[currentVisitorIndex].Exit();
            soundManager.PlayVisitor(villagers[currentVisitorIndex].sounds, VisitorSound.Exit);
            currentVisitorIndex = -1;
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