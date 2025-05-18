using UnityEngine;

namespace Buttons
{
    public class GridSelectablesHolder: SelectablesHolder
    {
        public override void OnIndexChange(int oldIndex, int newIndex)
        {
            base.OnIndexChange(oldIndex, newIndex);
            if (oldIndex < 0 || oldIndex > selectables.Length)
            {
                return;
            }

            int moveIndex = -1;
            SelectablesHolder holder = selectables[oldIndex] as SelectablesHolder;
            if (holder != null)
            {
                moveIndex = holder.CurrentIndex;
            }
            SelectablesHolder holder2 = selectables[newIndex] as SelectablesHolder;
            if (holder2 != null && moveIndex >= 0)
            {
                holder2.startIndex = Mathf.Min(moveIndex, holder2.selectables.Length);
            }
        }
    }
}