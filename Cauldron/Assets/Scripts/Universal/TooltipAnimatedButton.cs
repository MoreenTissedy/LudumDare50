using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Universal
{
    public class TooltipAnimatedButton: AnimatedButton
    {
        [SerializeField] private ScrollTooltip tooltip;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            tooltip.Open();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            tooltip.Close();
        }
    }
}