using Client.Common.AnimatorCallbackTool;

namespace Client.Common.AnimatorTools
{
    public interface IAnimatorCallbackReceiver
    {
        public void OnAnimationCallback(AnimatorCallbackInfo info);
    }
}