using CauldronCodebase;
using CauldronCodebase.GameStates;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndGameFX : MonoBehaviour
{
    [HideInInspector] public SoundManager SoundManager;
    [HideInInspector] public EndingScreen EndingScreen;
    [HideInInspector] public EndingsProvider EndingsProvider;
    [HideInInspector] public GameStateMachine GameStateMachine;
    [SerializeField] private Image endingIcon;
    [SerializeField] private float iconScaleDuration, iconFadeTime, iconFadeDelay;
    [SerializeField] private Animator animator;


    private void Start()
    {
        //endingIcon.sprite = EndingsProvider.Get(EndingsProvider.GetIndexOf(GameStateMachine.currentEnding)).endIconImage;
        
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(endingIcon.rectTransform.DOScale(1.1f, iconScaleDuration).SetEase(Ease.OutSine).SetSpeedBased())
                  .Append(endingIcon.rectTransform.DOScale(0.95f, iconScaleDuration/2).SetEase(Ease.InSine).SetSpeedBased())
                  .AppendCallback(PlayAnimation)
                  .AppendInterval(iconFadeDelay)
                  .Append(endingIcon.DOFade(0, iconFadeTime))
                  .AppendCallback(ShowEndingScreen);
    }

    private void PlayAnimation()
    {
        animator.enabled = true;
    }

    public void PlaySound()
    {
        SoundManager.Play(Sounds.GameEnd);
    }

    private void ShowEndingScreen()
    {
        EndingScreen.OpenBookWithEnding(GameStateMachine.currentEnding);
    }
}
