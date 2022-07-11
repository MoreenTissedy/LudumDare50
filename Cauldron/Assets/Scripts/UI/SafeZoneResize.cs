using System;
using UnityEngine;

namespace CauldronCodebase
{
    public class SafeZoneResize : MonoBehaviour
    {
        private void Start()
        {
            var thisRect = GetComponent<RectTransform>();
            Rect area = Screen.safeArea;
            thisRect.anchoredPosition = area.position;
            thisRect.sizeDelta = new Vector2(area.width-Screen.width, area.height - Screen.height);
        }
    }
}