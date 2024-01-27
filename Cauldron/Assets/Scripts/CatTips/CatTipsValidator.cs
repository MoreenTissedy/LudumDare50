using CauldronCodebase;
using UnityEngine;
using Zenject;

public class CatTipsValidator : MonoBehaviour
{
    private bool tipsWasShown;

    private CatAnimations catAnimations;
    private CatTipsView catTipsView;

    [Inject]
    private void Construct(CatTipsView tipsView, CatAnimations animations)
    {
        catTipsView = tipsView;
        catAnimations = animations;
    }

    public bool ShowTips(CatTips tips)
    {
        if (!catAnimations.OnStartSpot)
        {
            return false;
        }
        
        if (tipsWasShown)
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
