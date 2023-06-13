using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Save;
using UnityEngine;
using Zenject;

public class CatTipsProvider : MonoBehaviour, IDataPersistence
{
    private CatTipsManager catTipsManager;
    private GameStateMachine gameStateMachine;
    private GameDataHandler gameDataHandler;
    private MainSettings settings;
    private TooltipManager tooltipManager;
    private RecipeBook recipeBook;
    private IngredientsData ingredientsData;

    private bool DarkStrangerCame, WitchCame, InquisitorCame;

    [Inject]
    private void Construct(CatTipsManager tipsManager,
                            GameStateMachine stateMachine,
                            GameDataHandler dataHandler,
                            MainSettings mainSettings,
                            CatTipsView tipsView,
                            TooltipManager tooltip,
                            RecipeBook book,
                            IngredientsData ingredients,
                            DataPersistenceManager persistenceManager)
    {
        catTipsManager = tipsManager;
        gameStateMachine = stateMachine;
        gameDataHandler = dataHandler;
        settings = mainSettings;
        tooltipManager = tooltip;
        recipeBook = book;
        ingredientsData = ingredients;
        persistenceManager.AddToDataPersistenceObjList(this);
        
        gameStateMachine.OnChangeState += SubscribeOnChangeState;
    }
    
    private void OnDestroy()
    {
        gameStateMachine.OnChangeState -= SubscribeOnChangeState;
    }

    private void SubscribeOnChangeState(GameStateMachine.GamePhase phase)
    {
        if (phase != GameStateMachine.GamePhase.Visitor || gameDataHandler.currentCard.villager.name == "Cat")
        {
            StopAllCoroutines();
            return;
        }
        
        StartCoroutine(CheckTipsCondition(phase));
    }

    private IEnumerator CheckTipsCondition(GameStateMachine.GamePhase phase)
    {
        if(phase != GameStateMachine.GamePhase.Visitor) yield break;

        StartCoroutine(WaitSlowTips());

        yield return new WaitForSeconds(settings.catTips.VisitorCheckDelay);
        if (CheckSpecialVisitors()) yield break;

        yield return new WaitForSeconds(0.5f);
        CheckScale();
    }

    private IEnumerator WaitSlowTips()
    {
        yield return new WaitForSeconds(settings.catTips.SlowTipsDelay);
        
        if (tooltipManager.Highlighted || recipeBook.IsOpen)
        {
            yield break;
        }

        Ingredients[] randomRecipe;
        List<Ingredients[]> generatedRecipe = new List<Ingredients[]>(RecipeBook.MAX_COMBINATIONS_COUNT);
        int tryCount = 0;

        do
        {
            randomRecipe = recipeBook.GenerateRandomRecipe();

            if (generatedRecipe.Any(recipe => recipe.All(randomRecipe.Contains)))
            {
                continue;
            }

            tryCount++;
            if (tryCount >= RecipeBook.MAX_COMBINATIONS_COUNT)
            {
                yield break;
            }
            
        } while (recipeBook.IsIngredientSetKnown(randomRecipe));
        

        var ingredients = randomRecipe.Select(ingredient => ingredientsData.Get(ingredient)).ToList();
        
        catTipsManager.ShowTips(CatTipsGenerator.CreateTipsWithIngredients(catTipsManager.SlowPlayerTips, ingredients));
    }

    private bool CheckSpecialVisitors()
    {
        switch (gameDataHandler.currentCard.villager.name)
        {
            case "Inquisition":
                if (!InquisitorCame)
                {
                    catTipsManager.ShowTips(CatTipsGenerator.CreateRandomTips(catTipsManager.InquisitionTips));
                    InquisitorCame = true;
                    return true;
                }
                else
                    return false;
                
            case "DarkStranger":
                if (!DarkStrangerCame)
                {
                    catTipsManager.ShowTips(CatTipsGenerator.CreateRandomTips(catTipsManager.DarkStrangerTips));
                    DarkStrangerCame = true;
                    return true;
                }
                else
                    return false;
            
            case "WitchMemory":
                if (int.TryParse(gameDataHandler.currentCard.name[gameDataHandler.currentCard.name.Length-1].ToString(), out int index))
                {
                    catTipsManager.ShowTips(CatTipsGenerator.CreateSequencedTips(catTipsManager.WitchMemoryTips, index-1));
                    return true;
                }
                return false;
        }

        return false;
    }

    private void CheckScale()
    {
        var fame = gameDataHandler.Get(Statustype.Fame);
        var fear = gameDataHandler.Get(Statustype.Fear);
        
        CheckVisitor(gameDataHandler.currentCard.primaryInfluence);
        CheckVisitor(gameDataHandler.currentCard.secondaryInfluence);

        void CheckVisitor(Statustype status)
        {
            switch (status)
            {
                case Statustype.Fame:
                    if (fame >= gameDataHandler.GetThreshold(Statustype.Fame, true))
                    {
                        CreateTip(catTipsManager.HighFameTips, true);
                    }

                    if (fame <= gameDataHandler.GetThreshold(Statustype.Fame, false))
                    {
                        CreateTip(catTipsManager.LowFameTips, false);
                    }
                    break;
                case Statustype.Fear:
                    if (fear >= gameDataHandler.GetThreshold(Statustype.Fear, true))
                    {
                        CreateTip(catTipsManager.HighFearTips, true);
                    }

                    if (fear <= gameDataHandler.GetThreshold(Statustype.Fear, false))
                    {
                        CreateTip(catTipsManager.LowFearTips, false);
                    }
                    break;
            }
        }

        void CreateTip(CatTipsTextSO scaleText, bool high)
        {
            catTipsManager.ShowTips(gameDataHandler.currentCard.primaryCoef > 0
                ? CatTipsGenerator.CreateTips(scaleText, high ? catTipsManager.ScaleDOWNTips : catTipsManager.ScaleUPTips)
                : CatTipsGenerator.CreateTips(scaleText, high ? catTipsManager.ScaleUPTips : catTipsManager.ScaleDOWNTips));
            
        }
    }

    public void LoadData(GameData data, bool newGame)
    {
        DarkStrangerCame = data.DarkStrangerCame;
        WitchCame = data.WitchCame;
        InquisitorCame = data.InquisitorCame;
    }

    public void SaveData(ref GameData data)
    {
        data.DarkStrangerCame = DarkStrangerCame;
        data.WitchCame = WitchCame;
        data.InquisitorCame = InquisitorCame;
    }
}
