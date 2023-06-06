using CauldronCodebase;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

public class CatTipsManager : MonoBehaviour
{
    [Header("Tips")]
    public CatTipsTextSO SlowPlayerTips;
    public CatTipsTextSO RandomLastIngredient;

    [Header("Special character tips")]
    public CatTipsTextSO DarkStrangerTips;
    public CatTipsTextSO WitchMemoryTips;
    public CatTipsTextSO InquisitionTips;


    [Header("Scale tips")]
    public CatTipsTextSO HighFameTips;
    public CatTipsTextSO LowFameTips;
    public CatTipsTextSO HighFearTips;
    public CatTipsTextSO LowFearTips;
    [Space(10)]
    public CatTipsTextSO ScaleUPTips;
    public CatTipsTextSO ScaleDOWNTips;
    
    private bool tipsWasShown;

    private GameStateMachine gameStateMachine;
    private GameDataHandler gameDataHandler;
    private CatTipsView catTipsView;

    [Inject]
    private void Construct(GameStateMachine stateMachine, GameDataHandler dataHandler, CatTipsView tipsView)
    {
        gameStateMachine = stateMachine;
        gameDataHandler = dataHandler;
        catTipsView = tipsView;
    }

    private void Start()
    {
        gameStateMachine.OnChangeState += ReadyToShow;
    }

    private void OnDestroy()
    {
        gameStateMachine.OnChangeState -= ReadyToShow;
    }

    public void ShowTips(CatTips tips)
    {
        if(tipsWasShown) return;
        if(gameDataHandler.currentCard.villager.name == "Cat") return;
        
        tipsWasShown = true;
        
        catTipsView.ShowTips(tips);
    }

    private void ReadyToShow(GameStateMachine.GamePhase phase)
    {
        if(phase != GameStateMachine.GamePhase.Visitor) return;
        tipsWasShown = false;
    }
}
