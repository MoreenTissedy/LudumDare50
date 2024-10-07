using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class VisitorManager : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private CatVisitor witchCat;
        
        [SerializeField] private VisitorTextBox visitorText;
        [SerializeField] private VisitorTimer visitorTimer;
        [SerializeField] private ParticleSystem positiveReaction;
        [SerializeField] private ParticleSystem negativeReaction;

        public event Action VisitorLeft;
        public event Action VisitorEntering;

        public int attemptsLeft;
        private Visitor currentVisitor;
        private Villager currentVillager;
        public Villager CurrentVillager => currentVillager;
        private bool ignoreSavedAttempts = false;
        
        private Cauldron cauldron;
        private GameData gameData;
        private SoundManager soundManager;
        private DataPersistenceManager dataPersistenceManager;

        private VillagerFamiliarityChecker villagerFamiliarityChecker;

        [Inject]
        private void Init(Cauldron cauldron, DataPersistenceManager dataPersistenceManager, SoundManager soundManager, VillagerFamiliarityChecker villagerFamiliarityChecker)
        {
            this.soundManager = soundManager;
            this.cauldron = cauldron;
            dataPersistenceManager.AddToDataPersistenceObjList(this);
            this.dataPersistenceManager = dataPersistenceManager;
            this.villagerFamiliarityChecker = villagerFamiliarityChecker;
        }

        private void Awake()
        {
            HideText();
            cauldron.PotionDeclined += Wait;
        }

        private void ShowText(Encounter card)
        {
            if(visitorText == null) return;
            visitorText.Display(card);
        }

        private void HideText()
        {
            visitorText.Hide();
        }

        private void Wait()
        {
            if (!currentVisitor)
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
            Villager villager = card.villager;

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
            VisitorEntering?.Invoke();
            visitorTimer.ResetTimer(villager.patience);
            if (attemptsLeft != villager.patience)
            {
                for (int i = 0; i < villager.patience - attemptsLeft; i++)
                {
                    visitorTimer.ReduceTimer();
                }
            }

            currentVillager = villager;
            villagerFamiliarityChecker.TryAddVisitor(villager.name);
            
            // I couldn't think of a better way for regular visitors and the animated cat to work.
            if (villager.name == EncounterIdents.CAT)
            {
                currentVisitor = witchCat;
                currentVisitor.Enter();
                await UniTask.Delay((int)(witchCat.WalkingTime * 1000));
            }
            else
            {
                soundManager.PlayVisitor(villager.sounds, VisitorSound.Door);
                await UniTask.Delay(300);
                soundManager.PlayVisitor(villager.sounds, VisitorSound.Enter);
                currentVisitor = Instantiate(villager.visitorPrefab, transform);
                
                if (currentVisitor)
                {
                    currentVisitor.Enter();
                }
            }
            
            soundManager.PlayVisitor(villager.sounds, VisitorSound.Speech);

            await UniTask.Delay(150);
            ShowText(card);
        }

        public void Exit()
        {
            if(!currentVisitor) return;
            
            HideText();
            currentVisitor.ExitWithDestroy();
            soundManager.PlayVisitor(currentVillager.sounds, VisitorSound.Exit);
            currentVisitor = null;
            currentVillager = null;
        }

        public void PlayReaction(bool positive)
        {
            if (positive)
            {
                positiveReaction.Play();
            }
            else
            {
                negativeReaction.Play();
            }
        }

        public void LoadData(GameData data, bool newGame)
        {
            ignoreSavedAttempts = newGame;
            gameData = data;
            attemptsLeft = data.AttemptsLeft;
        }

        public void SaveData(ref GameData data)
        {
            if(data == null) return;
            
            data.AttemptsLeft = attemptsLeft;
        }
    }
}