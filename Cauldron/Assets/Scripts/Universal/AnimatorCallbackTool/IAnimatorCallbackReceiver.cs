using Client.Common.AnimatorCallbackTool;

namespace Client.Common.AnimatorTools
{
    public interface IAnimatorCallbackReceiver
    {
        void OnAnimationCallback(AnimatorCallbackInfo info);
    }
}