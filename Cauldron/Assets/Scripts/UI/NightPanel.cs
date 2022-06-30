using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        public NightPanelCard eventCard;
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

        [Header("DEBUG")]
        public NightEvent[] content;

        [Header("Interval between incoming cards in sec")] 
        public float enterTimeInterval = 1f;

        [Header("Card placement parameters")] public float angleDiff = -5f;
        public float angleReductionCoef = 0.3f;
        public Vector2 positionDiff = new Vector2(-10, -10);
        public float positionReductionCoef = 0.3f;
        public float firstCardAngle = -5f;

        private List<NightPanelCard> cardPool, activeCards;
        private Vector3 cardInitialPos, newCardPosition;
        private int eventCardSiblingIndex;
        private bool firstCardDealt;

        protected override void Awake()
        {
            if (eventCard is null)
            {
                Debug.LogError("Please specify a night panel card on nightPanel script");
                return;
            }
            cardPool = new List<NightPanelCard>(3) {eventCard};
            activeCards = new List<NightPanelCard>(3);
            cardInitialPos = eventCard.GetComponent<RectTransform>().anchoredPosition;
            newCardPosition = eventCard.transform.position;
            eventCardSiblingIndex = eventCard.transform.GetSiblingIndex();
            eventCard.gameObject.SetActive(false);
            base.Awake();
        }

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
            DOTween.To(() => flavour.alpha, x => flavour.alpha = x, 1, 1).From(0);
            money.text = "—";
            fear.text = "—";
            fame.text = "—";
        }

        public void OpenBookWithEvents(NightEvent[] events)
        {
            content = events;
            InitTotalPages();
            currentPage = 0;
            firstCardDealt = false;
            base.OpenBook();
            StartCoroutine(DealCards());
        }

        IEnumerator DealCards()
        {
            if (activeCards.Count > 0)
            {
                cardPool.AddRange(activeCards);
                activeCards.Clear();
            }
            Vector2 newPosition = cardInitialPos;
            Vector2 posDifference = positionDiff;
            float newAngle = firstCardAngle;
            float angleDifference = angleDiff;
            foreach (var nightEvent in content)
            {
                yield return new WaitForSeconds(enterTimeInterval);
                NightPanelCard card = GetCard();
                card.Init(nightEvent.picture, cardInitialPos);
                card.Enter(newPosition, newAngle);
                foreach (var activeCard in activeCards)
                {
                    activeCard.Punch();
                }
                newPosition += posDifference;
                newAngle += angleDifference;
                posDifference *= positionReductionCoef;
                angleDifference *= angleReductionCoef;
                card.InPlace += AddActiveCard;
            }
        }

        void AddActiveCard(NightPanelCard card)
        {
            card.InPlace -= AddActiveCard;
            if (!firstCardDealt)
            {
                firstCardDealt = true;
                UpdatePage();
            }
            activeCards.Add(card);
            //FanCards();
        }

        void FanCards()
        {
            Vector2 newPosition = cardInitialPos;
            Vector2 posDifference = positionDiff;
            float newAngle = 0;
            float angleDifference = angleDiff;
            for (var i = 0; i < activeCards.Count; i++)
            {
                activeCards[i].Move(newPosition, newAngle);
                newPosition += posDifference;
                newAngle += angleDifference;
                posDifference *= positionReductionCoef;
                angleDifference *= angleReductionCoef;
            }
        }

        NightPanelCard GetCard()
        {
            NightPanelCard newCard;
            if (cardPool.Count > 0)
            {
                newCard = cardPool[0];
                cardPool.RemoveAt(0);
                Debug.Log("card got from pool");
            }
            else
            {
                //copy nightPanelCard gameobject
                newCard = Instantiate(eventCard.gameObject, newCardPosition, Quaternion.identity, mainPanel).
                    GetComponent<NightPanelCard>();
                newCard.transform.SetSiblingIndex(eventCardSiblingIndex);
            }
            return newCard;
        }

        private void Show(NightEvent nightEvent)
        {
            flavour.text = nightEvent.flavourText;
            DOTween.To(() => flavour.alpha, x => flavour.alpha = x, 1, 1).From(0);
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
            if (content is null || content.Length == 0 || currentPage >= content.Length)
            {
                //TODO default events as events
                ShowDefault();
            }
            else if (firstCardDealt)
            {
                Show(content[currentPage]);
            }
            else
            {
                ClearContent();
            }
        }

        private void ClearContent()
        {
            flavour.text = String.Empty;
            money.text = String.Empty;
            fear.text = String.Empty;
            fame.text = String.Empty;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (currentPage + 1 < totalPages)
            {
                activeCards[0].Exit();
                cardPool.Add(activeCards[0]);
                activeCards.RemoveAt(0);
                FanCards();
                NextPage();
            }
            else
            {
                foreach (var nightPanelCard in activeCards)
                {
                    nightPanelCard.Hide();
                }
                CloseBook();
                //gm.StartNewDay();   
            }
        }
    }
}