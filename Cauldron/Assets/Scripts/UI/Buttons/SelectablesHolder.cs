using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Universal;

namespace Buttons
{
    public enum SelectableDirection
    {
        Horizontal,
        Vertical
    }

    public class SelectablesHolder : Selectable
    {
        [ReorderableList]
        public Selectable[] selectables;
        public SelectableDirection direction;

        private int currentIndex = -1;
        private Selectable Current => selectables[currentIndex];

        private void Reset()
        {
            selectables = GetComponentsInChildren<FlexibleButton>(false);
        }

        private void GetButtons()
        {
            if (direction == SelectableDirection.Horizontal)
            {
                selectables = selectables.OrderBy(x => x.transform.position.x).Where(x => x.gameObject.activeInHierarchy).ToArray();
            }
            else if (direction == SelectableDirection.Vertical)
            {
                selectables = selectables.OrderByDescending(x => x.transform.position.y).Where(x => x.gameObject.activeInHierarchy).ToArray();
            }
        }
        

        private void Update()
        {
            int diff = 0; //use proper Input
            if (direction == SelectableDirection.Vertical)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    diff = 1;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    diff = -1;
                }
            }

            if (direction == SelectableDirection.Horizontal)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    diff = 1;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    diff = -1;
                }
            }

            if (diff == 0)
            {
                return;
            }

            if (diff > 0 && currentIndex == selectables.Length - 1)
            {
                return;
            }

            if (diff < 0 && currentIndex == 0)
            {
                return;
            }

            Current.Unselect();
            currentIndex += diff;
            Current.Select();
        }

        public override void Select()
        {
            GetButtons();
            SelectDefaultElement();
        }

        protected virtual void SelectDefaultElement()
        {
            if (selectables is null || selectables.Length == 0) return;
            currentIndex = 0;
            Current.Select();
        }

        public override void Unselect()
        {
            Current.Unselect();
        }
    }
}