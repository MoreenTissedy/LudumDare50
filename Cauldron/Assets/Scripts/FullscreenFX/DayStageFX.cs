using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CauldronCodebase
{
    public class DayStageFX : BaseFX
    {
        [SerializeField] private TMP_Text tmpTextComponent;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Sounds sound;
        public float duration;

        public DayStageFX SetText(string text)
        {
            tmpTextComponent.text = text;
            return this;
        }
        
        public override UniTask Play()
        {
            Sound.Play(sound);
            return canvasGroup.DOFade(1, duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear).ToUniTask();
        }
    }
}
