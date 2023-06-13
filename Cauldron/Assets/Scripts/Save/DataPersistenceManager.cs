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

        public GameData GameSaveData => gameData;
        //TODO: Create global stat
        
        [SerializeField] private List<IDataPersistence> iDataPersistenceObj;
        private FileDataHandler fileDataHandler;

        private MainSettings settings;

        private bool newGame = false;
        public bool IsNewGame => newGame;

        [Inject]
        private void Construct(MainSettings mainSettings)
        {
            settings = mainSettings;
            fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        private void Awake()
        {
            if (CheckForGameSave())
            {
                gameData = fileDataHandler.Load();
                newGame = false;
            }
            else
            {
                NewGame();
            }
        }

        public void NewGame()
        {
            fileDataHandler.Delete();
            gameData = new GameData(settings.statusBars.InitialValue);
            newGame = true;
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

        private bool CheckForGameSave()
        {
            return PlayerPrefs.HasKey(FileDataHandler.PrefSaveKey) && fileDataHandler.IsSaveValid();
        }

        public void LoadDataPersistenceObj()
        {
            if(iDataPersistenceObj == null) return;
            foreach (var dataPersistenceObj in iDataPersistenceObj)
            {
                dataPersistenceObj.LoadData(gameData, newGame);
            }
        }
    }
}