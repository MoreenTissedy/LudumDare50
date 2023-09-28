using CauldronCodebase;
using UnityEngine;
using Zenject;

public class CatTipsValidator : MonoBehaviour
{
    private bool tipsWasShown;

    private GameDataHandler gameDataHandler;
    private CatTipsView catTipsView;

    [Inject]
    private void Construct(GameDataHandler dataHandler, CatTipsView tipsView)
    {
        gameDataHandler = dataHandler;
        catTipsView = tipsView;
    }

    public bool ShowTips(CatTips tips)
    {
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
