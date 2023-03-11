using System;
using System.Collections.Generic;
using CauldronCodebase;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public event Action OnPlayGame;

        [Inject]
        private void Construct(MainSettings mainSettings)
        {
            settings = mainSettings;
            fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        private void Awake()
        {
            gameData = fileDataHandler.Load();
            SceneManager.sceneLoaded += LoadDataPersistenceObj;
        }

        private void Start()
        {
            if (CheckTheExistenceOfGameData() == false)
            {
                NewGame();
            }
        }

        public void NewGame()
        {
            fileDataHandler.Delete();
            gameData = new GameData(settings.statusBars.InitialValue);
            Debug.Log("Create new game data");

            newGame = true;
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

        private void LoadDataPersistenceObj(Scene scene, LoadSceneMode mode)
        {
            if(scene.buildIndex != 1) return;
            if(iDataPersistenceObj == null) return;
            foreach (var dataPersistenceObj in iDataPersistenceObj)
            {
                dataPersistenceObj.LoadData(gameData, newGame);
            }
            PlayGame();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded += LoadDataPersistenceObj;
        }
    }
}