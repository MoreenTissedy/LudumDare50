using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CauldronCodebase
{
    public class DayStageFX : BaseFX
    {
        [SerializeField] private TMP_Text tmpTextComponent;
        [SerializeField] private string text;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Sounds sound;
        public float duration;

        public override UniTask Play()
        {
            Sound.Play(sound);
            tmpTextComponent.text = text;
            return canvasGroup.DOFade(1, duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear).ToUniTask();
        }
    }
}
