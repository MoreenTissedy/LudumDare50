using System;
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
                return GetButtonIndex(Mediator.GamepadButton);
            }

            return GetButtonIndex(Button);
        }

        private int GetButtonIndex(GamepadButton button)
        {
            switch (button)
            {
                case GamepadButton.None:
                    return -1;
                case GamepadButton.East:
                    return 0;
                case GamepadButton.West:
                    return 6;
                case GamepadButton.South:
                    return 1;
                case GamepadButton.North:
                    return 5;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
    }
}