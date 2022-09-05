using System;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class VisitorTextIcon : MonoBehaviour
    {
        public Image icon;
        public Sprite fame, fear, money, question, item;

        public void DisplayItem()
        {
            icon.sprite = item;
            gameObject.SetActive(true);
        }
        
        public void Display(Statustype type, bool hidden = false)
        {
            if (hidden)
            {
                icon.sprite = question;
                gameObject.SetActive(true);
                return;
            }
            switch (type)
            {
                case Statustype.None:
                    Hide();
                    break;
                case Statustype.Money:
                    gameObject.SetActive(true);
                    icon.sprite = money;
                    break;
                case Statustype.Fear:
                    gameObject.SetActive(true);
                    icon.sprite = fear;
                    break;
                case Statustype.Fame:
                    gameObject.SetActive(true);
                    icon.sprite = fame;
                    break;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}