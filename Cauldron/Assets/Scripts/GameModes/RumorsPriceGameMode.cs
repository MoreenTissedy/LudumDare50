using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/RumorsPrice")]
    public class RumorsPriceGameMode : GameModeBase
    {
        [SerializeField] private int modRumorPrice = -10;
        private GameDataHandler gameDataHandler;

        [Inject]
        public void Construct(GameDataHandler gameDataHandler)
        {
            this.gameDataHandler = gameDataHandler;
        }

        public override void Apply()
        {
            gameDataHandler.statusSettings.TryGetCovenCost += GetModPrice;
        }

        public int GetModPrice(int price)
        {
            return price + modRumorPrice;
        }
    }
}