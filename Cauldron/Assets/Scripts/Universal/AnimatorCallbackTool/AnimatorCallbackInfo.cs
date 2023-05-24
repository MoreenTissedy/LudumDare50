using Client.Common.AnimatorTools;
using UnityEngine;

namespace Client.Common.AnimatorCallbackTool
{
    public struct AnimatorCallbackInfo
    {
        public int TagHash;
        public int NameHash;
        public string Key;
        public AnimatorCallbackDispatcher.EventType Type;
        public Animator Dispatcher;
    }
}