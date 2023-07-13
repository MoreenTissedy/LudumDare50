using UnityEngine;
using UnityEngine.EventSystems;

namespace Universal
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ScrollTooltip tooltip;

        private void OnValidate()
        {
            if (!tooltip)
            {
                tooltip = GetComponentInChildren<ScrollTooltip>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.Open();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip.Close();
        }
    }
}