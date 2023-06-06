using System.Collections;
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
            Debug.Log("Stop cat tips provider coroutines");
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
        Debug.Log("Run slow tip");
        yield return new WaitForSeconds(settings.catTips.SlowTipsDelay);

        Debug.Log("Slow tip wait done");
        if (tooltipManager.Highlighted || recipeBook.IsOpen)
        {
            Debug.Log("Slow tips brake");
            yield break;
        }

        Ingredients[] randomRecipe;
        int tryCount = 0;

        do
        {
            randomRecipe = recipeBook.GenerateRandomRecipe();
            
            tryCount += 1;
            if(tryCount >= RecipeBook.MAX_COMBINATIONS_COUNT) yield break;
            
        } while (recipeBook.CheckRecipeIsOpen(randomRecipe));
        

        var ingredients = randomRecipe.Select(ingredient => ingredientsData.Get(ingredient)).ToList();
        
        catTipsManager.ShowTips(CatTipsGenerator.CreateTipsWithIngredients(catTipsManager.SlowPlayerTips, ingredients));
    }

    private bool CheckSpecialVisitors()
    {
        Debug.Log("Check special visitors");
        switch (gameDataHandler.currentCard.villager.name)
        {
            case "Inquisition":
                if (!InquisitorCame)
                {
                    catTipsManager.ShowTips(CatTipsGenerator.CreateTips(catTipsManager.InquisitionTips));
                    InquisitorCame = true;
                    return true;
                }
                else
                    return false;
                
            case "Dark Stranger":
                if (!DarkStrangerCame)
                {
                    catTipsManager.ShowTips(CatTipsGenerator.CreateTips(catTipsManager.DarkStrangerTips));
                    DarkStrangerCame = true;
                    return true;
                }
                else
                    return false;
            
            case "WitchMemory":
                if (!WitchCame)
                {
                    catTipsManager.ShowTips(CatTipsGenerator.CreateTips(catTipsManager.WitchMemoryTips));
                    WitchCame = true;
                    return true;
                }
                else
                    return false;
        }

        return false;
    }

    private void CheckScale()
    {
        Debug.Log("Check scale");
        var fame = gameDataHandler.Get(Statustype.Fame);
        var fear = gameDataHandler.Get(Statustype.Fear);

        if (fame > gameDataHandler.GetThreshold(Statustype.Fame, true))
        {
            CheckVisitor(Statustype.Fame, catTipsManager.HighFameTips, true);
            return;
        }

        if (fame < gameDataHandler.GetThreshold(Statustype.Fame, false))
        {
            CheckVisitor(Statustype.Fame, catTipsManager.LowFameTips, false);
            return;
        }

        if (fear > gameDataHandler.GetThreshold(Statustype.Fear, true))
        {
            CheckVisitor(Statustype.Fear, catTipsManager.HighFearTips, true);
            return;
        }

        if (fear < gameDataHandler.GetThreshold(Statustype.Fear, false))
        {
            CheckVisitor(Statustype.Fear, catTipsManager.LowFearTips, false);
        }

        void CheckVisitor(Statustype status, CatTipsTextSO scaleText, bool high)
        {
            if (gameDataHandler.currentCard.primaryInfluence == status)
            {
                catTipsManager.ShowTips(gameDataHandler.currentCard.primaryCoef > 0
                    ? CatTipsGenerator.CreateTips(scaleText, high ? catTipsManager.ScaleDOWNTips : catTipsManager.ScaleUPTips)
                    : CatTipsGenerator.CreateTips(scaleText, high ? catTipsManager.ScaleUPTips : catTipsManager.ScaleDOWNTips));
            }
            else if(gameDataHandler.currentCard.secondaryInfluence == status)
            {
                catTipsManager.ShowTips(gameDataHandler.currentCard.secondaryCoef > 0
                    ? CatTipsGenerator.CreateTips(scaleText, high ? catTipsManager.ScaleDOWNTips : catTipsManager.ScaleUPTips)
                    : CatTipsGenerator.CreateTips(scaleText, high ? catTipsManager.ScaleUPTips : catTipsManager.ScaleDOWNTips));
            }
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
