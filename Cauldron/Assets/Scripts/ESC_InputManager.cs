using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class ESC_InputManager : MonoBehaviour
    {
        //TODO: reactive UI
        private EndingScreen endingPanel;
        private RecipeBook recipeBook;
        private Tutorial tutorial;

        private bool debug;

        [Inject]
        private void Construct(EndingScreen endingPanel, RecipeBook recipeBook, Tutorial tutorial)
        {
            this.endingPanel = endingPanel;
            this.recipeBook = recipeBook;
            this.tutorial = tutorial;
        }
        
        private void Update()
        {
            //move to some sort of input manager?
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                endingPanel.Close();
                tutorial.CloseAllPages();
                if (recipeBook.bookObject.enabled)
                {
                    recipeBook.CloseBook();
                }
                else if (GameLoader.IsMenuOpen())
                {
                    GameLoader.UnloadMenu();
                }
                else
                {
                    GameLoader.LoadMenu();
                }
            }
        }
    }
}