using CauldronCodebase;
using UnityEngine;
using Zenject;

public class CatTipsValidator : MonoBehaviour
{
    private bool tipsWasShown;

    private CatAnimations catAnimations;
    private GameDataHandler gameDataHandler;
    private CatTipsView catTipsView;

    [Inject]
    private void Construct(GameDataHandler dataHandler, CatTipsView tipsView, CatAnimations animations)
    {
        gameDataHandler = dataHandler;
        catTipsView = tipsView;
        catAnimations = animations;
    }

    public bool ShowTips(CatTips tips)
    {
        if (catAnimations.IsDragged)
        {
            return false;
        }
        
        if (tipsWasShown)
        {
            return false;
        }

        if (gameDataHandler.currentCard.villager.name == EncounterIdents.CAT)
        {
            return false;
        }
        
        tipsWasShown = true;
        catTipsView.ShowTips(tips);
        return true;
    }

    public void HideTips()
    {
        if (tipsWasShown)
        {
            catTipsView.HideView();
            tipsWasShown = false;
        }
    }
}
