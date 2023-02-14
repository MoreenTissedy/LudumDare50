using UnityEngine;

namespace CauldronCodebase
{
    public abstract class VisitorTimer: MonoBehaviour
    {
        public abstract void ReduceTimer();
        public abstract void ResetTimer(int attempts);
    }
}