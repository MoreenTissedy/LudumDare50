using System;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public abstract class PrestigeGameMode : GameModeBase
    {
        [SerializeField] private int targetDays = 25;
        protected abstract string achievIdents { get; }
        
        protected GameStateMachine gameStates;
        protected GameDataHandler gameData;
        private IAchievementManager achievement;

        [Inject]
        public void Construct(GameStateMachine gameStates, GameDataHandler gameData, IAchievementManager achievement)
        {
            this.gameStates = gameStates;
            this.achievement = achievement;
            this.gameData = gameData;
        }
        
        public override void Apply()
        {
            gameStates.OnNewDay += TryUnlockAchivement;
            OnApply();
        }

        protected abstract void OnApply();

        private void TryUnlockAchivement()
        {
            if (gameData.currentDay == targetDays)
            {
                achievement.TryUnlock(achievIdents);
                Debug.Log($"Achivement {achievIdents} is unlock");
            }
        }

    }
}