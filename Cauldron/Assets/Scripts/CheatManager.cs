using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    //You can remove me from production
    public class CheatManager : MonoBehaviour
    {
        [Inject] private RecipeBook recipeBook;
        [Inject] private RecipeProvider recipeProvider;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private MainSettings settings;

        [Button("Unlock all recipes")]
        public void UnlockAllRecipes()
        {
            foreach (var recipe in recipeProvider.allRecipes)
            {
                if (recipeBook.IsRecipeInBook(recipe))
                {
                    continue;
                }
                recipeBook.RecordRecipe(recipe);
            }
        }
        
        [Button("Reset fame and fear")]
        public void ResetStatuses()
        {
            gameDataHandler.Fame = settings.statusBars.InitialValue;
            gameDataHandler.Fear = settings.statusBars.InitialValue;
        }

        [Button("Unlock coven & get rich")]
        public void UnlockCoven()
        {
            StoryTagHelper.SaveMilestone("circle");
            gameDataHandler.AddTag("circle");
            gameDataHandler.Money += 1000;
        }
       
        [Button("Enable auto cooking")]
        public void AutoCooking()
        {
            recipeBook.CheatUnlockAutoCooking();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.LogWarning(gameDataHandler.fractionStatus);
            }
        }
    }
}