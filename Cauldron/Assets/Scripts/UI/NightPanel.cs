using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class NightPanel : MonoBehaviour
    {
        public Text flavour;
        public Text money;
        public Text fear;
        public Text fame;

        private void Start()
        {
            Hide();
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