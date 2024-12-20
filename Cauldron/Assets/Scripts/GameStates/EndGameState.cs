﻿using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CauldronCodebase.GameStates
{
    public class EndGameState : BaseGameState
    {
        private readonly DataPersistenceManager dataPersistenceManager;
        private readonly GameFXManager gameFXManager;
        private readonly RecipeBook recipeBook;
        private readonly GameDataHandler gameDataHandler;

        private string currentEnding;
        private Transform root;
        private EndingScreen endingScreen;

        public EndGameState(DataPersistenceManager persistenceManager,
                            GameFXManager fxManager, RecipeBook recipeBook, GameDataHandler gameDataHandler)
        {
            dataPersistenceManager = persistenceManager;
            gameFXManager = fxManager;
            this.recipeBook = recipeBook;
            this.gameDataHandler = gameDataHandler;
        }
        public override void Enter()
        {
            recipeBook.CloseBook();
            ShowEffectAndEnding().Forget();
        }

        public void SetEnding(string tag, Transform root = null)
        {
            this.root = root;
            currentEnding = tag;
        }

        private async UniTaskVoid ShowEffectAndEnding()
        {
            var fx = gameFXManager.ShowEndGame(currentEnding);
            var loading = Resources.LoadAsync<EndingScreen>(ResourceIdents.EndingScreen);
            await UniTask.WhenAll(fx, loading.ToUniTask());
            endingScreen = Object.Instantiate(loading.asset, root) as EndingScreen;
            if (endingScreen != null)
            {
                endingScreen.Open(currentEnding);
                endingScreen.OnClose += ReloadGame;
            }
        }

        public override void Exit()
        {
            gameFXManager.Clear();
            if (endingScreen != null && endingScreen.isActiveAndEnabled)
            {
                endingScreen.Close();
            }
        }

        private void ReloadGame()
        {
            Debug.Log("reload scene");
            endingScreen.OnClose -= ReloadGame;
            dataPersistenceManager.NewGame();
            SceneManager.LoadScene(1);
        }
    }
}