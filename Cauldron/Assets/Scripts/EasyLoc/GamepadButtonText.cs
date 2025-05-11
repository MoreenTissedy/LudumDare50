using Buttons;
using CauldronCodebase;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace EasyLoc
{
    public class GamepadButtonText: MonoBehaviour, ILocTextTool
    {
        [Localize]
        public string FormatText;
        
        [BoxGroup("Choose one")]
        public GamepadButton Button;

        [BoxGroup("Choose one")]
        public GamepadMediator Mediator;

        public Color CharColor = Color.white;

        public TMP_Text TextField;

        private void Reset()
        {
            TextField = GetComponent<TMP_Text>();
        }

        public string GetId()
        {
            string id = gameObject.name;
            return id;
        }

        public void SetText(string text)
        {
            TextField.text = string.Format(text, $"<sprite={GetButton()} color={ColorToHex(CharColor)}>");
        }

        private static string ColorToHex(Color color)
        {
            return $"#{ColorUtility.ToHtmlStringRGB(color)}";
        }

        private int GetButton()
        {
            if (Mediator)
            {
                return (int)Mediator.GamepadButton;
            }

            return (int) Button;
        }
    }
}