using UnityEngine;

namespace Buttons
{
    public interface ISelectable
    {
        void Select();
        void Unselect();
    }

    //For Unity serialization
    public abstract class Selectable : MonoBehaviour, ISelectable
    {
        public abstract void Select();

        public abstract void Unselect();

        public abstract void Activate();
    }
}