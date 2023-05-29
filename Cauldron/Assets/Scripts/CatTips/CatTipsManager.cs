using System;
using System.Collections;
using System.Collections.Generic;
using CauldronCodebase.GameStates;
using UnityEngine;
using Zenject;

public class CatTipsManager : MonoBehaviour
{
    private bool tipsWasShown;

    private GameStateMachine gameStateMachine;

    [Inject]
    private void Construct(GameStateMachine stateMachine)
    {
        gameStateMachine = stateMachine;
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
        tipsWasShown = true;
        
        Debug.Log(tips.TipsText);
    }

    private void ReadyToShow(GameStateMachine.GamePhase phase)
    {
        if(phase != GameStateMachine.GamePhase.Visitor) return;
        tipsWasShown = false;
    }
}
