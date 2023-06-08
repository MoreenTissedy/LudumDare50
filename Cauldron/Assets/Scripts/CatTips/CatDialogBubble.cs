using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CatDialogBubble : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup text;
    [SerializeField] private float textFadeDuration;

    public void EnableBubble()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(PlayAnimation("cat_dialog"))
                .Append(text.DOFade(1, textFadeDuration).SetEase(Ease.InExpo))
                .AppendCallback(() => image.raycastTarget = true);
    }
    
    public void DisableBubble()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(PlayAnimation("cat_dialog_reverse"))
                .Append(text.DOFade(0, textFadeDuration).SetEase(Ease.OutExpo))
                .AppendCallback(() => image.raycastTarget = false);
    }

    private TweenCallback PlayAnimation(string animationName)
    {
        animator.Play(animationName);
        return null;
    }
}
