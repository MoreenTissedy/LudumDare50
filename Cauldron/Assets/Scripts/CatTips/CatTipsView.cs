using CauldronCodebase;
using TMPro;
using UnityEngine;
using Zenject;

public class CatTipsView : MonoBehaviour
{
    [SerializeField] private DialogIcon dialogIcon;
    [SerializeField] private CatDialogBubble catDialogBubble;
    [SerializeField] private TMP_Text text;
    
    [Inject] private SoundManager soundManager;

    private bool tipEnabled;
    
    public void ChangeTipView()
    {
        switch (tipEnabled)
        {
            case true:
                catDialogBubble.DisableBubble();
                dialogIcon.EnableIcon();
                tipEnabled = false;
                break;
            case false:
                catDialogBubble.EnableBubble();
                dialogIcon.DisableIcon();
                tipEnabled = true;
                break;
        }

    }

    public void ShowTips(CatTips tips)
    {
        text.text = tips.TipsText;
        tipEnabled = true;
        soundManager.PlayCat(CatSound.Attention);
        catDialogBubble.EnableBubble();
    }

    public void HideView()
    {
        dialogIcon.DisableIcon();
        if (tipEnabled)
        {
            catDialogBubble.DisableBubble();
        }
    }
}
