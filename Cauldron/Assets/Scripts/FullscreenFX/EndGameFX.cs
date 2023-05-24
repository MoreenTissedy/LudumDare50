using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class EndGameFX : BaseFX
    {
        [HideInInspector] public EndingsProvider EndingsProvider;
        [HideInInspector] public GameStateMachine GameStateMachine;
        [SerializeField] private Image endingIcon;

        [SerializeField] private float iconScaleDuration,
            iconAnimationDelay,
            iconFadeTime,
            iconFadeDelay,
            showEndingScreenDelay,
            showTextDelay,
            showTextDuration;

        [SerializeField] private Animator animator;
        [SerializeField] private Animator iconAnimator;

        [SerializeField] private CanvasGroup textCanvasGroup;
        [SerializeField] private TMP_Text text;

        private Ending currentEnding;

        public EndGameFX SelectEnding(Ending ending)
        {
            currentEnding = ending;
            return this;
        }
        public override UniTask Play()
        {
            //TODO: Add short text on all endings
            var animationGameObject = endingIcon.rectTransform.parent;

            endingIcon.sprite = currentEnding.endIconImage;
            //text.text = ending.shortTextForEndingAnimation;

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(
                    animationGameObject.DOScale(1.1f, iconScaleDuration).SetEase(Ease.OutSine).SetSpeedBased())
                .Append(animationGameObject.DOScale(0.95f, iconScaleDuration / 2).SetEase(Ease.InSine).SetSpeedBased())
                .AppendCallback(PlayAnimation)
                .AppendInterval(iconAnimationDelay)
                .AppendCallback(PlayIconAnimation)
                .AppendInterval(iconFadeDelay)
                .AppendCallback(DoIconFillAmount)
                //.AppendInterval(showTextDelay)
                //.Append(textCanvasGroup.DOFade(1, showTextDuration))
                .AppendInterval(showEndingScreenDelay * 2);
            //.AppendCallback(ShowEndingScreen);
            return mySequence.ToUniTask();
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
            Sound.Play(Sounds.GameEnd);
        }
    }
}
