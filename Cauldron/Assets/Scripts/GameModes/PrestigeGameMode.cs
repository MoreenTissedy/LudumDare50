using System;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class PrestigeGameMode : GameModeBase
    {
        [SerializeField] private int targetDays = 25;
        protected string achievIdents;
        
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
        }

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