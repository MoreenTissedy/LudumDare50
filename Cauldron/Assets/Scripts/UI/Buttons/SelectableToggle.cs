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
        
        public override void Select()
        {
            initialColor = label.color;
            label.color = highlightColor;
            inputManager.Controls.General.AnyKey.performed += Toggle;
        }

        private void Toggle(InputAction.CallbackContext context)
        {
            toggle.isOn = !toggle.isOn;
        }

        public override void Unselect()
        {
            label.color = initialColor;
            inputManager.Controls.General.AnyKey.performed -= Toggle;
        }
    }
}