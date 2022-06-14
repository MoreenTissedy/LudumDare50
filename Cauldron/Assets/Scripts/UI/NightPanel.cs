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

            Show(text, 0, 0, 0);
        }

        public void Show(string fl, int m, int fr, int fm)
        {
            flavour.text = fl;
            money.text = m.ToString();
            fear.text = fr.ToString();
            fame.text = fm.ToString();
            gameObject.SetActive(true);
        }
        

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}