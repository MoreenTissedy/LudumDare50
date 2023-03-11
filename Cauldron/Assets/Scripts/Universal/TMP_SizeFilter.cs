using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Universal
{
    public class TMP_SizeFilter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;

        [SerializeField] private LayoutElement layoutElement;

        [SerializeField] private float scalePercent;
        
        [ContextMenu("SetHeight")]
        public void SetHeight()
        {
            if(textMeshPro == null) return;

            float coeff = textMeshPro.preferredHeight / 100 * scalePercent;

            layoutElement.preferredHeight = textMeshPro.preferredHeight - coeff;
        }

        private void Start()
        {
            SetHeight();
        }

        private void Update()
        {
            if (layoutElement.preferredHeight != textMeshPro.preferredHeight)
            {
                SetHeight();
            }
        }
    }
}