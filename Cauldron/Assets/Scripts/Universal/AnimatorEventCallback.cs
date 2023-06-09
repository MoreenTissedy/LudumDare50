using JetBrains.Annotations;
using UnityEngine;

namespace Universal
{
    public class AnimatorEventCallback: MonoBehaviour
    {
        public bool callbackReceived;

        private void OnDisable()
        {
            callbackReceived = false;
        }

        [UsedImplicitly]
        public void Callback()
        {
            callbackReceived = true;
        }
    }
}