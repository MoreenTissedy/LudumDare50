using System;
using System.Linq;
using System.Threading;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class CatTipsProvider : MonoBehaviour
{
    private CatTipsManager catTipsManager;
    private GameStateMachine gameStateMachine;
    private GameDataHandler gameDataHandler;
    private MainSettings settings;
    private TooltipManager tooltipManager;
    private RecipeBook recipeBook;
    private IngredientsData ingredientsData;
    private CancellationTokenSource cancellationTokenSource;

    private bool DarkStrangerCame, WitchCame, InquisitorCame;

    [Inject]
    private void Construct(CatTipsManager tipsManager,
                            GameStateMachine stateMachine,
                            GameDataHandler dataHandler,
                            MainSettings mainSettings,
                            CatTipsView tipsView,
                            TooltipManager tooltip,
                            RecipeBook book,
                            IngredientsData ingredients)
    {
        catTipsManager = tipsManager;
        gameStateMachine = stateMachine;
        gameDataHandler = dataHandler;
        settings = mainSettings;
        tooltipManager = tooltip;
        recipeBook = book;
        ingredientsData = ingredients;
    }

    private void Start()
    {
        cancellationTokenSource = new CancellationTokenSource();
        gameStateMachine.OnChangeState += SubscribeOnChangeState;
    }

    private void OnDestroy()
    {
        gameStateMachine.OnChangeState -= SubscribeOnChangeState;
    }

    private void SubscribeOnChangeState(GameStateMachine.GamePhase phase)
    {
        if (phase != GameStateMachine.GamePhase.Visitor)
        {
            cancellationTokenSource?.Cancel();
            return;
        }
        
        cancellationTokenSource = new CancellationTokenSource();
        CheckTipsCondition(phase).AttachExternalCancellation(cancellationTokenSource.Token);
    }

    private async UniTask CheckTipsCondition(GameStateMachine.GamePhase phase)
    {
        if(phase != GameStateMachine.GamePhase.Visitor) return;
        
        WaitSlowTips().AttachExternalCancellation(cancellationTokenSource.Token);

        await UniTask.Delay(TimeSpan.FromSeconds(settings.catTips.VisitorCheckDelay));
        if (CheckSpecialVisitors()) return;

        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        CheckScale();
    }

    private async UniTask WaitSlowTips()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(settings.catTips.SlowTipsDelay));
        if(tooltipManager.Highlighted && recipeBook.IsOpen) return;

        Ingredients[] randomRecipe;

        do
        {
            randomRecipe = recipeBook.GenerateRandomRecipe();
        } while (recipeBook.CheckRecipeIsOpen(randomRecipe));

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
}
