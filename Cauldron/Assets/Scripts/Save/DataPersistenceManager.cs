using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")] 
        [SerializeField] private string fileName;
        
        private GameData gameData;
        
        private List<IDataPersistence> iDataPersistenceObj;
        private FileDataHandler<GameData> fileDataHandler;

        private MainSettings settings;
        private SODictionary soDictionary;
        private PlayerProgressProvider progressProvider;

        public bool IsNewGame { get; private set; }

        [Inject]
        private void Construct(MainSettings mainSettings, SODictionary dictionary, PlayerProgressProvider progressProvider)
        {
            settings = mainSettings;
            soDictionary = dictionary;
            fileDataHandler = new FileDataHandler<GameData>(fileName);
            this.progressProvider = progressProvider;
        }

        private void Awake()
        {
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
            gameData = new GameData(settings.statusBars.InitialValue, progressProvider.GetMilestones());
            IsNewGame = true;
        }

        public void SaveGame()
        {
            foreach (var dataPersistenceObj in iDataPersistenceObj)
            {
                dataPersistenceObj.SaveData(ref gameData);
            }
            
            fileDataHandler.Save(gameData);
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