using CauldronCodebase;
using CauldronCodebase.GameStates;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameFX : MonoBehaviour
{
    [HideInInspector] public SoundManager SoundManager;
    [HideInInspector] public EndingScreen EndingScreen;
    [HideInInspector] public EndingsProvider EndingsProvider;
    [HideInInspector] public GameStateMachine GameStateMachine;
    [SerializeField] private Image endingIcon;
    [SerializeField] private float iconScaleDuration, iconAnimationDelay,
                                    iconFadeTime, iconFadeDelay, showEndingScreenDelay, 
                                    showTextDelay, showTextDuration;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator iconAnimator;

    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private TMP_Text text;


    private void Start()
    {
        //TODO: Add short text on all endings
        
        var ending = EndingsProvider.Get(EndingsProvider.GetIndexOf(GameStateMachine.currentEnding));
        var animationGameObject = endingIcon.rectTransform.parent;
        
        endingIcon.sprite = ending.endIconImage;
        //text.text = ending.shortTextForEndingAnimation;
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(animationGameObject.DOScale(1.1f, iconScaleDuration).SetEase(Ease.OutSine).SetSpeedBased())
                  .Append(animationGameObject.DOScale(0.95f, iconScaleDuration/2).SetEase(Ease.InSine).SetSpeedBased())
                  .AppendCallback(PlayAnimation)
                  .AppendInterval(iconAnimationDelay)
                  .AppendCallback(PlayIconAnimation)
                  .AppendInterval(iconFadeDelay)
                  .AppendCallback(DoIconFillAmount)
                  //.AppendInterval(showTextDelay)
                  //.Append(textCanvasGroup.DOFade(1, showTextDuration))
                  .AppendInterval(showEndingScreenDelay * 2)
                  .AppendCallback(ShowEndingScreen);
    }

    private void PlayAnimation()
    {
        animator.enabled = true;
    }

    private void PlayIconAnimation()
    {
        iconAnimator.enabled = true;
    }

    private void DoIconFillAmount()
    {
        endingIcon.DOFillAmount(0, iconFadeTime).SetEase(Ease.InOutQuad);
    }

    public void PlaySound()
    {
        SoundManager.Play(Sounds.GameEnd);
    }

    private void ShowEndingScreen()
    {
        Debug.Log("show");
        EndingScreen.OpenBookWithEnding(GameStateMachine.currentEnding);
    }
}
