using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler

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

        [Inject]
        private RecipeBook book;

        private void Start()
        {
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
            transf.DOSizeDelta((initialScale * sizeCoef), sizeSpeed).
                SetLoops(-1, LoopType.Yoyo);
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

        public void OnPointerClick(PointerEventData eventData)
        {
            clicked = true;
            transf.DOKill(true);
            book.OpenBook();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //grow in size
            if (!clicked)
                transf.DOPause();
            transf.DOSizeDelta((initialScale * sizeCoef), sizeSpeed);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //shrink in size
            if (clicked)
                transf.DOSizeDelta((initialScale), sizeSpeed);
            else
            {
                transf.DOPlay();
            }
        }
    }
}