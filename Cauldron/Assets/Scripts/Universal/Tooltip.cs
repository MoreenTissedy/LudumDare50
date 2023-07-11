using UnityEngine;
using UnityEngine.EventSystems;

namespace Universal
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ScrollTooltip tooltip;

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