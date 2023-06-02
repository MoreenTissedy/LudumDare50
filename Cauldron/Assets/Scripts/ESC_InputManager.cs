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

                if (recipeBook.bookObject.enabled)
                {
                    recipeBook.CloseBook();
                }
                else if (endingPanel.bookObject.enabled)
                {
                    endingPanel.CloseBook();
                }
                else if (tutorial.canvas.enabled)
                {
                    tutorial.CloseAllPages();
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