using UnityEngine;

namespace Buttons
{
    public class GridSelectablesHolder: SelectablesHolder
    {
        protected override bool TryChangeIndex(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex > Selectables.Length)
            {
                return false;
            }

            int moveIndex = -1;
            SelectablesHolder holder = Selectables[oldIndex] as SelectablesHolder;
            if (holder != null)
            {
                moveIndex = holder.CurrentIndex;
            }
            SelectablesHolder holder2 = Selectables[newIndex] as SelectablesHolder;
            if (holder2 != null && moveIndex >= 0)
            {
                holder2.startIndex = Mathf.Min(moveIndex, holder2.Selectables.Length);
            }

            return true;
        }
    }
}