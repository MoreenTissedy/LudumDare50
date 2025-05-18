using UnityEngine;

namespace Universal
{
    public class TooltipButtonComponent: AnimatedButtonComponent
    {
        [SerializeField] private ScrollTooltip tooltip;

        public override void Activate(){}

        public override void ChangeInteractive(bool isInteractive){}

        public override void Select()
        {
            tooltip.Open();
        }

        public override void Unselect()
        {
            tooltip.Close();
        }
    }
}