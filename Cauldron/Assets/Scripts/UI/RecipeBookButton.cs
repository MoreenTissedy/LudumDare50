using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CauldronCodebase
{
    public class RecipeBookButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler

    {
        public float sizeCoef = 1.2f;
        public float sizeSpeed = 0.2f;
        private bool clicked;
        private Vector3 initialScale;
        private RectTransform transf;

        private void Start()
        {
            transf = GetComponent<RectTransform>();
            initialScale = transf.sizeDelta;
            //start flashing to attract attention
                transf.DOSizeDelta((initialScale * sizeCoef), sizeSpeed).
                    SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            clicked = true;
            transf.DOKill(true);
            RecipeBook.instance.OpenBook();
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