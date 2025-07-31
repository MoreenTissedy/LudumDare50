using TMPro;
using UnityEngine;
using Universal;

namespace Buttons
{
    public class TextColorAnimation: AnimatedButtonComponent
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Color highlightColor;
        
        private Color initialColor;
        public override void Select()
        {
            initialColor = label.color;
            label.color = highlightColor;
        }

        public override void Unselect()
        {
            label.color = initialColor;
        }

        public override void Activate()
        {
            
        }

        public override void ChangeInteractive(bool isInteractive)
        {
            
        }
    }
}