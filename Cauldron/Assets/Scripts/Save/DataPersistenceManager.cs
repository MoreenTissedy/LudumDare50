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
        [SerializeField] private List<IDataPersistence> iDataPersistenceObj;
        private FileDataHandler fileDataHandler;

        private MainSettings settings;

        private bool newGame;

        [Inject]
        private void Construct(MainSettings mainSettings)
        {
            settings = mainSettings;
            fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        private void Start()
        {
            gameData = fileDataHandler.Load();
            GameLoader.OnGameLoadComplete += LoadDataPersistenceObj;
        }

        public void NewGame()
        {
            gameData = new GameData(settings.statusBars.InitialValue);

            newGame = true;
        }

        public void LoadGame()
        {
            if (gameData == null)
            {
                NewGame();
            }

            newGame = false;
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
        }

        private void OnDestroy()
        {
            GameLoader.OnGameLoadComplete -= LoadDataPersistenceObj;
        }
    }
}