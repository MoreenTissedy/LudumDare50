using UnityEngine;

namespace Universal
{
    public abstract class AnimatedButtonComponent : MonoBehaviour
    {
        public abstract void Select();
        public abstract void Unselect();
        public abstract void Activate();
        public abstract void ChangeInteractive(bool isInteractive);
    }
}