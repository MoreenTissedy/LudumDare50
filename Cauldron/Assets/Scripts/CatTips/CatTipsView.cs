using System;
using System.Threading;
using CauldronCodebase;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class CatTipsView : MonoBehaviour
{
    private const int _SHOW_TIME_ = 3;
    
    [SerializeField] private DialogIcon dialogIcon;
    [SerializeField] private CatDialogBubble catDialogBubble;
    [SerializeField] private TMP_Text text;
    
    [Inject] private SoundManager soundManager;

    private bool tipEnabled;
    private CancellationTokenSource cancel;
    
    public void ChangeTipView()
    {
        switch (tipEnabled)
        {
            case true:
                ChangeToIcon();
                break;
            case false:
                ChangeToBubble();
                break;
        }

    }

    private void ChangeToIcon()
    {
        catDialogBubble.DisableBubble();
        dialogIcon.EnableIcon();
        tipEnabled = false;
    }

    private void ChangeToBubble()
    {
        catDialogBubble.EnableBubble();
        dialogIcon.DisableIcon();
        tipEnabled = true;
    }

    public async void ShowTips(CatTips tips)
    {
        cancel?.Cancel();
        cancel = new CancellationTokenSource();
        text.text = tips.TipsText;
        soundManager.PlayCat(CatSound.Attention);
        ChangeToBubble();
        await UniTask.Delay(TimeSpan.FromSeconds(_SHOW_TIME_)).
            AttachExternalCancellation(cancel.Token).SuppressCancellationThrow();
        if (tipEnabled)
        {
            ChangeToIcon();
        }
    }

    public void HideView()
    {
        cancel?.Cancel();
        dialogIcon.DisableIcon();
        if (tipEnabled)
        {
            catDialogBubble.DisableBubble();
        }
    }
}
