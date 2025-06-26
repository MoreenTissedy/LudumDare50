using Client.Common.AnimatorCallbackTool;
using Client.Common.AnimatorTools;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CauldronCodebase
{
    public class StartGameFX : BaseFX, IAnimatorCallbackReceiver
    {
        private bool animationEnded;

        public float hintDuration = 3f;
        public float hintFadeFuration = 1f;
        public GameObject hintBlock;

        [SerializeField] private TMP_Text tmpTextComponent;

        public override UniTask Play()
        {
            return UniTask.WaitUntil(() => animationEnded);
        }

        public void OnAnimationCallback(AnimatorCallbackInfo info)
        {
            animationEnded = true;
        }

        public StartGameFX SetSavingProcessHint(string text)
        {
            hintBlock.SetActive(true);
            tmpTextComponent.text = text;
            hintBlock.AddComponent<CanvasGroup>().DOFade(0, hintFadeFuration).SetDelay(hintDuration);
            return this;
        }
    }
}
