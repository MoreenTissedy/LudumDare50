using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class GameManager : MonoBehaviour
    {
        //TODO: reactive UI
        public EndingScreen endingPanel;
        [Inject] private RecipeBook recipeBook;
        
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