using CauldronCodebase.GameStates;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class SimpleNumber : MonoBehaviour
    {
        public Statustype type;
        private Text text;
        
        private GameDataHandler gameDataHandler;

        [Inject]
        private void Construct(GameDataHandler dataHandler)
        {
            gameDataHandler = dataHandler;
        }
        private void Start()
        {
            text = GetComponent<Text>();
            gameDataHandler.StatusChanged += (x, y) => SetValue(gameDataHandler.Get(type));
            SetValue(gameDataHandler.Get(type));
        }

        void SetValue(int value)
        {
            text.text = value.ToString();
        }
    }
}