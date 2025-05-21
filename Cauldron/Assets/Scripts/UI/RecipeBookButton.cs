using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookButton : AnimatedButtonComponent
    {
        public float sizeCoef = 1.2f;
        public float sizeSpeed = 0.2f;
        private bool clicked;
        private Vector3 initialScale;
        private RectTransform transf;

        private Canvas canvas;
        private GraphicRaycaster raycaster;
        private RectTransform bgButtonTransf;
        private float initialPosXButton;
        private float offPosXButton;
        public float openCloseAnimationTime = 0.4f;
        public GameObject gamePadIcon;

        [Inject]
        private RecipeBook book;

        public bool flashing;

        private void Start()
        {
            book.hudButton = this;
            
            transf = GetComponent<RectTransform>();
            initialScale = transf.sizeDelta;

            canvas = GetComponentInParent<Canvas>();
            raycaster = GetComponentInParent<GraphicRaycaster>();
            bgButtonTransf = canvas.gameObject.GetComponent<RectTransform>();

            initialPosXButton = bgButtonTransf.anchoredPosition.x;
            offPosXButton = initialPosXButton + 210;
        }

        private void OnEnable()
        {
            //start flashing to attract attention
            transf.DOSizeDelta(initialScale * sizeCoef, sizeSpeed).
                SetLoops(-1, LoopType.Yoyo);
            gamePadIcon.SetActive(true);
            flashing = true;
        }

        private void Update()
        {
            if (flashing && book.IsOpen)
            {
                flashing = false;
                gamePadIcon.SetActive(false);
                transf.DOKill();
            }
        }

        public void ChangeLayer(string name, int order)
        {
            canvas.sortingLayerName = name;            
            canvas.sortingOrder = order;
        }

        public void ChangeBookAvailable(bool isAvailable)
        {
            Sequence mySequence = DOTween.Sequence();
            if(isAvailable)
            {
                mySequence.AppendCallback(
                    () => raycaster.enabled = true).
                    Append(bgButtonTransf.DOAnchorPosX(initialPosXButton, openCloseAnimationTime)).
                    Append(transf.DOSizeDelta(initialScale, openCloseAnimationTime * 2).SetEase(Ease.InOutBack));
            }
            else
            {
                mySequence.AppendCallback(
                    () => raycaster.enabled = false).
                    Append(transf.DOSizeDelta(Vector2.zero, openCloseAnimationTime * 2).SetEase(Ease.InOutBack)).
                    Append(bgButtonTransf.DOAnchorPosX(offPosXButton, openCloseAnimationTime));
            }
        }

        public override void Select()
        {
            //grow in size
            if (!clicked)
                transf.DOPause();
            transf.DOSizeDelta(initialScale * sizeCoef, sizeSpeed);
        }

        public override void Unselect()
        {
            //shrink in size
            if (clicked)
                transf.DOSizeDelta(initialScale, sizeSpeed);
            else
            {
                transf.DOPlay();
            }
        }

        public override void Activate()
        {
            clicked = true;
            transf.DOKill(true);
            book.OpenBook();
            gamePadIcon.SetActive(false);
        }

        public override void ChangeInteractive(bool isInteractive){}
    }
}