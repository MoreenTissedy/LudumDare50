using Client.Common.AnimatorCallbackTool;
using Client.Common.AnimatorTools;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CauldronCodebase
{
    public class StartGameFX : BaseFX, IAnimatorCallbackReceiver
    {
        private bool animationEnded;

        public override UniTask Play()
        {
            return UniTask.WaitUntil(() => animationEnded);
        }

        public void OnAnimationCallback(AnimatorCallbackInfo info)
        {
            animationEnded = true;
        }
    }
}
