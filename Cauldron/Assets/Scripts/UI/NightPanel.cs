using System;
using EasyLoc;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class NightPanel : MonoBehaviour
    {
        public Text flavour;
        public Text money;
        public Text fear;
        public Text fame;
        [Localize]
        public string defaultNightText1 = "Ничего необычного.";
        [Localize]
        public string defaultNightText2 = "Ночь спокойна и тиха.";
        [Localize]
        public string defaultNightText3 = "Дует ветер, гонит тучки.";

        private void Start()
        {
            Hide();
        }

        public void Show(NightEvent[] events)
        {
            if (events is null || events.Length == 0)
            {
                ShowDefault();
            }
            else
            {
                Show(events[0]);
            }
            //show multiple events
        }

        public void ShowDefault()
        {
            string text = String.Empty;
            int rnd = Random.Range(0, 3);
            switch (rnd)
            {
                case 0:
                    text = defaultNightText1;
                    break;
                case 1:
                    text = defaultNightText2;
                    break;
                case 2:
                    text = defaultNightText3;
                    break;
            }

            flavour.text = text;
            money.text = "—";
            fear.text = "—";
            fame.text = "—";
            gameObject.SetActive(true);
        }
        
        

        public void Show(NightEvent nightEvent)
        {
            flavour.text = nightEvent.flavourText;
            money.text = nightEvent.moneyModifier.ToString();
            fear.text = nightEvent.fearModifier.ToString();
            fame.text = nightEvent.fameModifier.ToString();
            gameObject.SetActive(true);
        }
        

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}