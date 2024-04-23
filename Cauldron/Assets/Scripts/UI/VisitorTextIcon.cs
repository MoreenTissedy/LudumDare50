using System;
using Cysharp.Threading.Tasks;
using EasyLoc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal;

namespace CauldronCodebase
{
    public class VisitorTextIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Localize] public string fearHint = "Если вы поможете или навредите, изменится шкала страха.";
        [Localize] public string moneyHint = "Этот персонаж даст вам денег, если вы поможете ему.";
        [Localize] public string fameHint = "Если вы поможете или навредите, изменится шкала славы.";
        [Localize] public string fractionHint = "Ваше решение будет замечено и оценено по достоинству.";
        
        public Image icon;
        public Sprite fame, fear, money, question, bishop, king, bandit;
        public ScrollTooltip hint;

        private bool hintEnabled;

        public void DisplayFraction(Fractions fraction)
        {
            if (fraction is Fractions.None)
            {
                return;
            }
            hintEnabled = false;
            gameObject.SetActive(true);
            switch (fraction)
            {
                case Fractions.King:
                    icon.sprite = king;
                    break;
                case Fractions.Bishop:
                    icon.sprite = bishop;
                    break;
                case Fractions.Rogue:
                    icon.sprite = bandit;
                    break;
            }
            hint.SetText(fractionHint).ContinueWith(() => hintEnabled = true);
        }
        
        public void Display(Statustype type, bool hidden = false)
        {
            hintEnabled = false;
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
                    hint.SetText(moneyHint).ContinueWith(() => hintEnabled = true);
                    break;
                case Statustype.Fear:
                    gameObject.SetActive(true);
                    icon.sprite = fear;
                    hint.SetText(fearHint).ContinueWith(() => hintEnabled = true);
                    break;
                case Statustype.Fame:
                    gameObject.SetActive(true);
                    hint.SetText(fameHint).ContinueWith(() => hintEnabled = true);
                    icon.sprite = fame;
                    break;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hintEnabled)
            {
                hint.Open();
            } 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hint.Close();
        }
    }
}