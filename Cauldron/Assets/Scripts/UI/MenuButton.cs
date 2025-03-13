using UnityEngine;
using Universal;

namespace CauldronCodebase
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField] private FlexibleButton button;

        private void Reset()
        {
            button = GetComponent<FlexibleButton>();
        }
        
        public void Start()
        {
            button.OnClick += OnClick;
        }

        public void OnClick()
        {
            if (GameLoader.IsMenuOpen())
            {
                GameLoader.UnloadMenu();
            }
            else
            {
                GameLoader.LoadMenu();
            }
        }

        private void OnDestroy()
        {
            button.OnClick -= OnClick;
        }
    }
}