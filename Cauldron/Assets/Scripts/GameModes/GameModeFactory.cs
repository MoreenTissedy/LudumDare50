using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameModeFactory
    {
        private readonly DiContainer container; //This is a bad practice, don't use it anywhere))
        private List<GameModeBase> appliedGameModes;

        [Inject]
        public GameModeFactory(Wardrobe wardrobe, DiContainer container)
        {
            wardrobe.SkinApplied += TryApplyGameMode;
            this.container = container;
        }

        private void TryApplyGameMode(SkinSO obj)
        {
            GameModeBase gameMode = obj.GameMode;
            if (!gameMode)
            {
                return;
            }
            
            container.Inject(gameMode);
            
            if (appliedGameModes is null)
            {
                appliedGameModes = new List<GameModeBase>();
            }
            appliedGameModes.Add(gameMode);
            
            gameMode.Apply();
            Debug.Log("Apply game mode "+gameMode.name);
        }
    }
}