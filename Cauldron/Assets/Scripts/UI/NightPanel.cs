using System;
using DG.Tweening;
using EasyLoc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class NightPanel : Book, IPointerClickHandler
    {
        [Inject]
        private GameManager gm;
        
        public TMP_Text flavour;
        public TMP_Text money;
        public TMP_Text fear;
        public TMP_Text fame;
        [Localize]
        public string defaultNightText1 = "Ничего необычного.";
        [Localize]
        public string defaultNightText2 = "Ночь спокойна и тиха.";
        [Localize]
        public string defaultNightText3 = "Дует ветер, гонит тучки.";

        public NightEvent[] content;

        private void ShowDefault()
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
            //flavour.DOFade(1, 1).From(0);
            money.text = "—";
            fear.text = "—";
            fame.text = "—";
        }

        public void OpenBookWithEvents(NightEvent[] events)
        {
            content = events;
            InitTotalPages();
            base.OpenBook();
        }

        private void Show(NightEvent nightEvent)
        {
            flavour.text = nightEvent.flavourText;
            //flavour.DOFade(1, 1).From(0);
            money.text = nightEvent.moneyModifier.ToString();
            fear.text = nightEvent.fearModifier.ToString();
            fame.text = nightEvent.fameModifier.ToString();
        }

        protected override void InitTotalPages()
        {
            totalPages = content.Length;
        }

        protected override void UpdatePage()
        {
            Debug.Log("night panel update");
            if (content is null || content.Length == 0 || currentPage >= content.Length)
            {
                ShowDefault();
            }
            else
            {
                Show(content[currentPage]);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (currentPage + 1 < totalPages)
            {
                NextPage();
            }
            else
            {
                CloseBook();
                gm.StartNewDay();   
            }
        }
    }
}