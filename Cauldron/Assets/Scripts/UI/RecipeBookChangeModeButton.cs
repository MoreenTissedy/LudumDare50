using UnityEngine;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookChangeModeButton : MonoBehaviour
    {
        [SerializeField] private FlexibleButton button;

        public RecipeBook.Mode mode;
        
        [Inject]
        private RecipeBook recipeBook;

        public void Awake()
        {
            button.OnClick += OnClick;
        }

        public void OnClick()
        {
            recipeBook.ChangeMode(mode);
        }

        public void OnDestroy()
        {
            button.OnClick -= OnClick;
        }
    }
}