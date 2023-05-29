using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;


public class CatTipsProvider : MonoBehaviour
{
    [SerializeField] private List<string> slowPlayerTips;

    [Header("Special character tips")]
    [SerializeField] private List<string> BlackStrangerTips;
    [SerializeField] private List<string> WitchMemoryTips;
    [SerializeField] private List<string> InquisitionTips;


    [Header("Scale tips")]
    [SerializeField] private List<string> highFameTips;
    [SerializeField] private List<string> lowFameTips;
    [SerializeField] private List<string> highFearTips;
    [SerializeField] private List<string> lowFearTips;

    private CatTipsManager catTipsManager;
    private GameStateMachine gameStateMachine;
    private GameDataHandler gameDataHandler;
    private MainSettings settings;

    [Inject]
    private void Construct(CatTipsManager tipsManager,
                            GameStateMachine stateMachine,
                            GameDataHandler dataHandler,
                            MainSettings mainSettings)
    {
        catTipsManager = tipsManager;
        gameStateMachine = stateMachine;
        gameDataHandler = dataHandler;
        settings = mainSettings;
    }

    private void Start()
    {
        gameStateMachine.OnChangeState += SubscribeOnChangeState;
    }

    private void OnDestroy()
    {
        gameStateMachine.OnChangeState -= SubscribeOnChangeState;
    }

    private void SubscribeOnChangeState(GameStateMachine.GamePhase phase)
    {
        CheckTipsCondition(phase).Forget();
    }

    private async UniTaskVoid CheckTipsCondition(GameStateMachine.GamePhase phase)
    {
        if(phase != GameStateMachine.GamePhase.Visitor) return;

        await UniTask.Delay(TimeSpan.FromSeconds(1));
        if (CheckSpecialVisitors()) return;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        if(CheckScale()) return;
    }

    private bool CheckSpecialVisitors()
    {
        if (VisitorManager.SPECIALS.Contains(gameDataHandler.currentCard.villager.name))
        {
            switch (gameDataHandler.currentCard.villager.name)
            {
                case "Inquisition":
                    CreateSimpleTips(InquisitionTips);
                    return true;
                case "Dark Stranger":
                    CreateSimpleTips(BlackStrangerTips);
                    return true;
            }
        }

        return false;
    }

    private bool CheckScale()
    {
        var fame = gameDataHandler.Get(Statustype.Fame);
        var fear = gameDataHandler.Get(Statustype.Fear);

        if (fame >= gameDataHandler.GetThreshold(Statustype.Fame, true))
        {
            CreateSimpleTips(highFameTips);
            return true;
        }

        if (fame <= gameDataHandler.GetThreshold(Statustype.Fame, false))
        {
            CreateSimpleTips(lowFameTips);
            return true;
        }

        if (fear >= gameDataHandler.GetThreshold(Statustype.Fear, true))
        {
            CreateSimpleTips(highFearTips);
            return true;
        }

        if (fear <= gameDataHandler.GetThreshold(Statustype.Fear, false))
        {
            CreateSimpleTips(lowFearTips);
            return true;
        }

        return false;
    }

    private void CreateSimpleTips(List<string> tipsVariantList)
    {
        int random = Random.Range(0, tipsVariantList.Count);

        CatTips tips = new CatTips(tipsVariantList[random]);
        catTipsManager.ShowTips(tips);
    }
}
