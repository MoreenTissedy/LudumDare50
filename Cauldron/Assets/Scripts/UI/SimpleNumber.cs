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
            gm.gState.statusChanged += () => SetValue(gm.gState.Get(type));
            SetValue(gm.gState.Get(type));
        }

        void SetValue(int value)
        {
            text.text = value.ToString();
        }
    }
}