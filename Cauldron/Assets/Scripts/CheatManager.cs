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
        [Inject] private EndingsProvider endingsProvider;

        [Button("Unlock all endings")]
        public void UnlockAllEndings()
        {
            endingsProvider.TryUnlock(EndingsProvider.FINAL);
            endingsProvider.TryUnlock(EndingsProvider.BANDIT);
            endingsProvider.TryUnlock(EndingsProvider.END_DECK);
            endingsProvider.TryUnlock(EndingsProvider.KING_BAD);
            endingsProvider.TryUnlock(EndingsProvider.LOW_FAME);
            endingsProvider.TryUnlock(EndingsProvider.LOW_FEAR);
            endingsProvider.TryUnlock(EndingsProvider.HIGH_FAME);
            endingsProvider.TryUnlock(EndingsProvider.HIGH_FEAR);
            endingsProvider.TryUnlock(EndingsProvider.KING_GOOD);
            endingsProvider.TryUnlock(EndingsProvider.BISHOP_BAD);
            endingsProvider.TryUnlock(EndingsProvider.BISHOP_GOOD);
            endingsProvider.TryUnlock(EndingsProvider.ENOUGH_MONEY);
        }
        
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