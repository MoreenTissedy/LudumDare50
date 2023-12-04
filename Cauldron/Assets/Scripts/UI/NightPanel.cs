using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyLoc;
using UnityEngine;
using Zenject;
using TMPro;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class NightPanel : Book
    {
        [SerializeField] private NightPanelCard eventCard;
        [SerializeField] private RectTransform cardRoot;
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
        public List<NightEvent> content;

        [Header("Coven features")] 
        public GameObject covenBlock;
        public CovenPopupButton covenButton;
        public GameObject covenPopup;
        
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

        private GameDataHandler gameDataHandler;
        private EventResolver resolver;
        private bool clickable;
        
        Vector2 lastPosition;
        Vector2 lastPositionDifference;
        float lastAngle;
        float lastAngleDifference;

        public event Action<NightEvent> EventClicked;

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
            eventCard.gameObject.SetActive(false);
            base.Awake();
        }

        [Inject]
        private void Construct(MainSettings settings, GameDataHandler gameDataHandler)
        {
            resolver = new EventResolver(settings, gameDataHandler);
            this.gameDataHandler = gameDataHandler;
        }

        private void OnEnable()
        {
            bool covenFeatureUnlocked = StoryTagHelper.CovenFeatureUnlocked(gameDataHandler);
            covenBlock.SetActive(covenFeatureUnlocked);
            covenPopup.SetActive(false);
            if (covenFeatureUnlocked)
            {
                covenButton.OnClick += CovenButtonOnOnClick;
            }
        }

        private void OnDisable()
        {
            covenButton.OnClick -= CovenButtonOnOnClick;
        }

        private void CovenButtonOnOnClick()
        {
            covenPopup.SetActive(!covenPopup.activeSelf);
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
            content = events.ToList();
            InitTotalPages();
            currentPage = 0;
            firstCardDealt = false;
            base.OpenBook();
            StartCoroutine(DealCards());
        }

        IEnumerator DealCards()
        {
            clickable = false;
            if (activeCards.Count > 0)
            {
                cardPool.AddRange(activeCards);
                activeCards.Clear();
            }
            lastPosition = cardInitialPos;
            lastPositionDifference = positionDiff;
            lastAngle = firstCardAngle;
            lastAngleDifference = angleDiff;
            foreach (var nightEvent in content)
            {
                yield return new WaitForSeconds(enterTimeInterval);
                DealEventCard(nightEvent);
            }
            yield return new WaitForSeconds(enterTimeInterval * 2);
            clickable = true;
        }

        private void DealEventCard(NightEvent nightEvent)
        {
            NightPanelCard card = GetCard();
            card.Init(nightEvent.picture, cardInitialPos, SoundManager);
            card.Enter(lastPosition, lastAngle);
            foreach (var activeCard in activeCards)
            {
                activeCard.Punch();
            }
            lastPosition += lastPositionDifference;
            lastAngle += lastAngleDifference;
            lastPositionDifference *= positionReductionCoef;
            lastAngleDifference *= angleReductionCoef;
            card.InPlace += AddActiveCard;
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

        public async void AddCovenEvent(NightEvent nightEvent)
        {
            clickable = false;
            covenPopup.SetActive(false);
            content.Add(nightEvent);
            DealEventCard(nightEvent);
            await UniTask.Delay(TimeSpan.FromSeconds(enterTimeInterval));
            FanCards();
            await UniTask.Delay(TimeSpan.FromSeconds(enterTimeInterval));
            clickable = true;
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
                newCard = Instantiate(eventCard.gameObject, newCardPosition, Quaternion.identity, cardRoot).
                    GetComponent<NightPanelCard>();
            }
            return newCard;
        }

        private void Show(NightEvent nightEvent)
        {
            flavour.text = nightEvent.flavourText;
            DOTween.To(() => flavour.alpha, x => flavour.alpha = x, 1, 1).From(0);
            money.text = resolver.CalculateModifier(Statustype.Money, nightEvent).ToString();
            fear.text = resolver.CalculateModifier(Statustype.Fear, nightEvent).ToString();
            fame.text = resolver.CalculateModifier(Statustype.Fame, nightEvent).ToString();
        }

        protected override void InitTotalPages()
        {
            totalPages = content.Count;
        }

        protected override void UpdatePage()
        {
            if (firstCardDealt)
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

        public async void OnNextCard()
        {
            if (content.Count == 0)
            {
                CloseBook();
            }
            EventClicked?.Invoke(content[currentPage]);
            if (currentPage + 1 < totalPages)
            {
                clickable = false;
                activeCards[0].Exit();
                cardPool.Add(activeCards[0]);
                activeCards.RemoveAt(0);
                FanCards();
                NextPage();
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                clickable = true;
            }
            else
            {
                foreach (var nightPanelCard in activeCards)
                {
                    nightPanelCard.Hide();
                }

                CloseBook();
            }
        }
    }
}