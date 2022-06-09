using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class VisitorTextBox : MonoBehaviour
    {
        public Text text;
        public Image icon1,icon2;
        public Sprite fame, fear, money;


        public void Display(Encounter card)
        {
            text.text = card.text;
            switch (card.primaryInfluence)
            {
                case Statustype.None:
                    icon1.enabled = false;
                    break;
                case Statustype.Money:
                    icon1.enabled = true;
                    icon1.sprite = money;
                    break;
                case Statustype.Fear:
                    icon1.enabled = true;
                    icon1.sprite = fear;
                    break;
                case Statustype.Fame:
                    icon1.enabled = true;
                    icon1.sprite = fame;
                    break;
            }
            switch (card.secondaryInfluence)
            {
                case Statustype.None:
                    icon2.enabled = false;
                    break;
                case Statustype.Money:
                    icon2.enabled = true;
                    icon2.sprite = money;
                    break;
                case Statustype.Fear:
                    icon2.enabled = true;
                    icon2.sprite = fear;
                    break;
                case Statustype.Fame:
                    icon2.enabled = true;
                    icon2.sprite = fame;
                    break;
            }
        }
    }
}