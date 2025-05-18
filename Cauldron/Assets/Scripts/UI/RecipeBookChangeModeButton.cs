using UnityEngine;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookChangeModeButton : MonoBehaviour
    {
        [SerializeField] private FlexibleButton button;

        public RecipeBook.Mode mode;
        
        [Inject] private RecipeBook recipeBook;
        [Inject] private InputManager inputManager;

        public void Awake()
        {
            button.OnClick += OnClick;
            recipeBook.OnModeChanged += OnBookModeChanged;
        }

        private void OnBookModeChanged(RecipeBook.Mode newMode)
        {
            if (!inputManager.GamepadConnected)
            {
                return;
            }
            if (mode == newMode)
            {
                button.Select();
            }
            else
            {
                button.Unselect();
            }
        }

        private void OnClick()
        {
            recipeBook.ChangeMode(mode);
        }

        public void OnDestroy()
        {
            button.OnClick -= OnClick;
            recipeBook.OnModeChanged -= OnBookModeChanged;
        }
    }
}