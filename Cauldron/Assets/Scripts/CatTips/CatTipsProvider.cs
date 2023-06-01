using System;
using System.Threading;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public class CatTipsProvider : MonoBehaviour
{
    [SerializeField] private CatTipsTextSO slowPlayerTips;

    [Header("Special character tips")]
    [SerializeField] private CatTipsTextSO DarkStrangerTips;
    [SerializeField] private CatTipsTextSO WitchMemoryTips;
    [SerializeField] private CatTipsTextSO InquisitionTips;


    [Header("Scale tips")]
    [SerializeField] private CatTipsTextSO highFameTips;
    [SerializeField] private CatTipsTextSO lowFameTips;
    [SerializeField] private CatTipsTextSO highFearTips;
    [SerializeField] private CatTipsTextSO lowFearTips;
    [Space(10)]
    [SerializeField] private CatTipsTextSO ScaleUPTips;
    [SerializeField] private CatTipsTextSO ScaleDOWNTips;

    private CatTipsManager catTipsManager;
    private GameStateMachine gameStateMachine;
    private GameDataHandler gameDataHandler;
    private MainSettings settings;
    private TooltipManager tooltipManager;
    private CancellationTokenSource cancellationTokenSource;

    private bool DarkStrangerCame, WitchCame, InquisitorCame;

    [Inject]
    private void Construct(CatTipsManager tipsManager,
                            GameStateMachine stateMachine,
                            GameDataHandler dataHandler,
                            MainSettings mainSettings,
                            CatTipsView tipsView,
                            TooltipManager tooltip)
    {
        catTipsManager = tipsManager;
        gameStateMachine = stateMachine;
        gameDataHandler = dataHandler;
        settings = mainSettings;
        tooltipManager = tooltip;
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
        if(CheckScale()) return;
    }

    private async UniTask WaitSlowTips()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(settings.catTips.SlowTipsDelay));
        if(tooltipManager.Highlighted) return;
        catTipsManager.ShowTips(CatTips.CreateTips(slowPlayerTips));
    }

    private bool CheckSpecialVisitors()
    {
        if (VisitorManager.SPECIALS.Contains(gameDataHandler.currentCard.villager.name))
        {
            switch (gameDataHandler.currentCard.villager.name)
            {
                case "Inquisition":
                    if (!InquisitorCame)
                    {
                        catTipsManager.ShowTips(CatTips.CreateTips(InquisitionTips));
                        InquisitorCame = true;
                        return true;
                    }
                    else
                        return false;
                    
                case "Dark Stranger":
                    if (!DarkStrangerCame)
                    {
                        catTipsManager.ShowTips(CatTips.CreateTips(DarkStrangerTips));
                        DarkStrangerCame = true;
                        return true;
                    }
                    else
                        return false;
                    
            }
        }

        return false;
    }

    private bool CheckScale()
    {
        var fame = gameDataHandler.Get(Statustype.Fame);
        var fear = gameDataHandler.Get(Statustype.Fear);

        if (fame > gameDataHandler.GetThreshold(Statustype.Fame, true))
        {
            CheckVisitor(Statustype.Fame, highFameTips, true);
            return true;
        }

        if (fame < gameDataHandler.GetThreshold(Statustype.Fame, false))
        {
            CheckVisitor(Statustype.Fame, lowFameTips, false);
            return true;
        }

        if (fear > gameDataHandler.GetThreshold(Statustype.Fear, true))
        {
            CheckVisitor(Statustype.Fear, highFearTips, true);
            return true;
        }

        if (fear < gameDataHandler.GetThreshold(Statustype.Fear, false))
        {
            CheckVisitor(Statustype.Fear, lowFearTips, false);
            return true;
        }
        
        return false;

        void CheckVisitor(Statustype status, CatTipsTextSO scaleText, bool high)
        {
            if (gameDataHandler.currentCard.primaryInfluence == status)
            {
                if (gameDataHandler.currentCard.primaryCoef > 0)
                {
                    catTipsManager.ShowTips(CatTips.CreateTips(scaleText, high ? ScaleDOWNTips : ScaleUPTips));
                }
                else
                {
                    catTipsManager.ShowTips(CatTips.CreateTips(scaleText, high ? ScaleUPTips : ScaleDOWNTips));
                }
            }
            else if(gameDataHandler.currentCard.secondaryInfluence == status)
            {
                if (gameDataHandler.currentCard.secondaryCoef > 0)
                {
                    catTipsManager.ShowTips(CatTips.CreateTips(scaleText, high ? ScaleDOWNTips : ScaleUPTips));
                }
                else
                {
                    catTipsManager.ShowTips(CatTips.CreateTips(scaleText, high ? ScaleUPTips : ScaleDOWNTips));
                }
            }
        }
    }
}
