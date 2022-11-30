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
        
        private GameData gameData;

        [Inject]
        private void Construct(StateMachine stateMachine)
        {
            gameData = stateMachine.GameData;
        }
        private void Start()
        {
            text = GetComponent<Text>();
            gameData.StatusChanged += () => SetValue(gameData.Get(type));
            SetValue(gameData.Get(type));
        }

        void SetValue(int value)
        {
            text.text = value.ToString();
        }
    }
}