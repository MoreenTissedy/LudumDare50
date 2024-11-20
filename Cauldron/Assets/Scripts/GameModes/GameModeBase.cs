using UnityEngine;

namespace CauldronCodebase
{
    public abstract class GameModeBase : ScriptableObject
    {
        public abstract void Apply();

        public virtual bool ShouldReapply { get; } = true;
    }
}