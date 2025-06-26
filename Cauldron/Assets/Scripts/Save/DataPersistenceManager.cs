using System;
using System.Collections.Generic;
using System.Threading;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")] 
        [SerializeField] private string fileName;

        [Header("Animation")] 
        [SerializeField] private GameObject animation;
        [SerializeField] private float animationMinTime = 1f;
        
        private GameData gameData;
        
        private List<IDataPersistence> iDataPersistenceObj;
        private FileDataHandler<GameData> fileDataHandler;

        private MainSettings settings;
        private SODictionary soDictionary;
        private MilestoneProvider milestones;

        public bool IsNewGame { get; private set; }

        private CancellationTokenSource cts;

        [Inject]
        private void Construct(MainSettings mainSettings, SODictionary dictionary, MilestoneProvider milestones)
        {
            settings = mainSettings;
            soDictionary = dictionary;
            fileDataHandler = new FileDataHandler<GameData>(fileName, false);
            this.milestones = milestones;
        }

        private void Awake()
        {
            animation.SetActive(false);
            if (IsSaveFound())
            {
                gameData = fileDataHandler.Load();
                gameData.ValidateSave(soDictionary);
                IsNewGame = false;
                
                if (gameData.Phase == GameStateMachine.GamePhase.EndGame)
                {
                    NewGame();
                }
            }
            else
            {
                NewGame();
            }
        }

        public void NewGame()
        {
            fileDataHandler.Delete();
            milestones.LoadMilestones();
            gameData = new GameData(settings.statusBars.InitialValue, milestones.Milestones);
            IsNewGame = true;
        }

        public void SaveGame()
        {
            foreach (var dataPersistenceObj in iDataPersistenceObj)
            {
                dataPersistenceObj.SaveData(ref gameData);
            }
            
            fileDataHandler.Save(gameData);
            SetSaveAnimation().Forget();
        }

        private async UniTaskVoid SetSaveAnimation()
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            animation.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(animationMinTime), DelayType.UnscaledDeltaTime,
                cancellationToken: cts.Token);
            animation.SetActive(false);
        }

        public void AddToDataPersistenceObjList(IDataPersistence obj)
        {
            if (iDataPersistenceObj == null)
            {
                iDataPersistenceObj = new List<IDataPersistence>(6);
            }
            if (iDataPersistenceObj.Contains(obj)) 
            {
                return;
            }
            iDataPersistenceObj.Add(obj);
        }

        public bool IsSaveFound()
        {
            return fileDataHandler.IsFileValid();
        }

        public void LoadDataPersistenceObj()
        {
            if(iDataPersistenceObj == null) return;
            foreach (var dataPersistenceObj in iDataPersistenceObj)
            {
                dataPersistenceObj.LoadData(gameData, IsNewGame);
            }
        }
    }
}