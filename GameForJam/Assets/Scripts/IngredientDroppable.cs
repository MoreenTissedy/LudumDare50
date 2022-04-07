using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class IngredientDroppable: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Ingredients type;
        public float rotateAngle = 10f;
        public float rotateSpeed = 0.3f;
        public IngredientsData dataList;
        
        
        [SerializeField, HideInInspector]
        private Text tooltip;
        [SerializeField, HideInInspector]
        private SpriteRenderer image;

        private void OnValidate()
        {
            tooltip = GetComponentInChildren<Text>();
            if (tooltip != null)
            {
                tooltip.text = dataList?.Get(type)?.friendlyName ?? "not specified";
            }

            image = GetComponentInChildren<SpriteRenderer>();
            image.sprite = dataList?.Get(type)?.image;
        }

        private void Start()
        {
            if (tooltip != null)
            {
                tooltip.gameObject.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Cauldron.instance.mix.Contains(type))
                return;
            image.transform.DOKill(true);
            Cauldron.instance.AddToMix(type);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(true);
            if (!Cauldron.instance.mix.Contains(type))
                image.gameObject.transform.
                    DORotate(new Vector3(0,0, rotateAngle), rotateSpeed).
                    SetLoops(-1, LoopType.Yoyo).
                    From(new Vector3(0, 0, -rotateAngle)).
                    SetEase(Ease.InOutSine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(false);
            image.transform.DOKill();
            image.transform.DORotate(new Vector3(0, 0, 0), rotateSpeed);
        }
    }
}