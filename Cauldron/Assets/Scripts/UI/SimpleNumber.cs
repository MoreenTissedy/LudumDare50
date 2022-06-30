using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class SimpleNumber : MonoBehaviour
    {
        public Statustype type;
        private Text text;

        [Inject]
        private GameManager gm;
        private void Start()
        {
            text = GetComponent<Text>();
            gm.GameState.StatusChanged += () => SetValue(gm.GameState.Get(type));
            SetValue(gm.GameState.Get(type));
        }

        void SetValue(int value)
        {
            text.text = value.ToString();
        }
    }
}