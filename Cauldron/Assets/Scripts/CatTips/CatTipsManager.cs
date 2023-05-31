using CauldronCodebase;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

public class CatTipsManager : MonoBehaviour
{
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
        if(tipsWasShown || gameDataHandler.currentCard.villager.name == "Cat") return;
        
        tipsWasShown = true;
        
        catTipsView.ShowTips(tips);

    }

    private void ReadyToShow(GameStateMachine.GamePhase phase)
    {
        catTipsView.HideView();
        if(phase != GameStateMachine.GamePhase.Visitor) return;
        tipsWasShown = false;
    }
}
