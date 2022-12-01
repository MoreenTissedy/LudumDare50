using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class ESC_InputManager : MonoBehaviour
    {
        //TODO: reactive UI
        private EndingScreen endingPanel;
        private RecipeBook recipeBook;

        [Inject]
        private void Construct(EndingScreen endingScreen, RecipeBook recipe)
        {
            endingPanel = endingScreen;
            recipeBook = recipe;
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
                else
                {
                    GameLoader.LoadMenu();
                }
            }
        }
    }
}