using System;
using System.Collections.Generic;
using CauldronCodebase;
using UnityEngine;
using Zenject;

namespace Save
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")] 
        [SerializeField] private string fileName;
        
        private GameData gameData;
        //TODO: Create global stat
        
        [SerializeField] private List<IDataPersistence> iDataPersistenceObj;
        private FileDataHandler fileDataHandler;

        private MainSettings settings;

        private bool newGame;

        public event Action OnPlayGame;
        public event Action OnDontExistSave;

        [Inject]
        private void Construct(MainSettings mainSettings)
        {
            settings = mainSettings;
            fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        private void Awake()
        {
            gameData = fileDataHandler.Load();
            GameLoader.OnGameLoadComplete += LoadDataPersistenceObj;
        }

        private void Start()
        {
            if (CheckTheExistenceOfGameData() == false)
            {
                NewGame();
            }
            LoadDataPersistenceObj();
        }

        public void NewGame()
        {
            gameData = new GameData(settings.statusBars.InitialValue);

            newGame = true;
            
            OnDontExistSave?.Invoke();
        }

        public void PlayGame()
        {
            OnPlayGame?.Invoke();
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
            if(iDataPersistenceObj.Contains(obj)) return;
            iDataPersistenceObj.Add(obj);
        }

        public bool CheckTheExistenceOfGameData()
        {
            return gameData != null;
        }

        private void LoadDataPersistenceObj()
        {
            if(iDataPersistenceObj == null) return;
            foreach (var dataPersistenceObj in iDataPersistenceObj)
            {
                dataPersistenceObj.LoadData(gameData, newGame);
            }
            PlayGame();
        }

        private void OnDestroy()
        {
            GameLoader.OnGameLoadComplete -= LoadDataPersistenceObj;
        }
    }
}