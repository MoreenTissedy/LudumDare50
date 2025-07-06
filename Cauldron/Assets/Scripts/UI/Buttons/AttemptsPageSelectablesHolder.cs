using UnityEngine;

namespace Buttons
{
    public sealed class AttemptsPageSelectablesHolder: GridSelectablesHolder
    {
        //At indices 0, 0 & 0, 1 filter block is situated
        protected override bool TryChangeIndex(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex > Selectables.Length)
            {
                return false;
            }
            SelectablesHolder holder = Selectables[oldIndex] as SelectablesHolder;
            if (holder != null && oldIndex == 0 & (holder.CurrentIndex == 0 || holder.CurrentIndex == 1))
            {
                return false;
            }

            int moveIndex = -1;
            if (holder != null)
            {
                moveIndex = holder.CurrentIndex;
            }
            SelectablesHolder holder2 = Selectables[newIndex] as SelectablesHolder;
            if (holder2 != null && moveIndex >= 0)
            {
                if (oldIndex == 0)
                {
                    holder2.startIndex = moveIndex - 1;
                }
                else if (newIndex == 0)
                {
                    holder2.startIndex = moveIndex + 1;
                }
                else
                {
                    holder2.startIndex = moveIndex;
                }
            }

            return true;
        }

        protected override bool TryActivate()
        {
            if (CurrentIndex > 0)
            {
                return true;
            }
            SelectablesHolder holder = Selectables[0] as SelectablesHolder;
            if (holder != null && (holder.CurrentIndex == 0 || holder.CurrentIndex == 1))
            {
                return false;
            }
            return true;
        }
    }
}