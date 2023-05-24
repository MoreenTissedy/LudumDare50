using Client.Common.AnimatorCallbackTool;
using Client.Common.AnimatorTools;
using Cysharp.Threading.Tasks;

namespace CauldronCodebase
{
    public class StartGameFX : BaseFX, IAnimatorCallbackReceiver
    {
        private bool animationEnded;

        public override UniTask Play()
        {
            Sound.Play(Sounds.GameStart);
            return UniTask.WaitUntil(() => animationEnded);
        }

        public void OnAnimationCallback(AnimatorCallbackInfo info)
        {
            animationEnded = true;
        }
    }
}
