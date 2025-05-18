using CauldronCodebase;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Buttons
{
    public class SelectableToggle: Selectable
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Color highlightColor;
        
        [Inject] private InputManager inputManager;

        private Color initialColor;
        private float lastInputTime;
        
        public override void Select()
        {
            initialColor = label.color;
            label.color = highlightColor;
            inputManager.Controls.General.AnyKey.performed += Toggle;
        }

        private void Toggle(InputAction.CallbackContext context)
        {
            if (Time.realtimeSinceStartup - lastInputTime < 0.3f)
            {
                return;
            }

            toggle.isOn = !toggle.isOn;
            lastInputTime = Time.realtimeSinceStartup;
        }

        public override void Unselect()
        {
            label.color = initialColor;
            inputManager.Controls.General.AnyKey.performed -= Toggle;
        }
    }
}