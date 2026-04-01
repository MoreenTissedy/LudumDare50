using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogIcon : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image image;
    [SerializeField] private float duration;

    private bool active;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void EnableIcon()
    {
        if (active)
        {
            return;
        }
        active = true;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, duration * 2).SetEase(Ease.InExpo).OnComplete((() => image.raycastTarget = true));
    }

    public void DisableIcon()
    {
        if (!active)
        {
            return;
        }
        active = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, duration).SetEase(Ease.OutExpo).OnComplete(() =>image.raycastTarget = false);
    }
}
