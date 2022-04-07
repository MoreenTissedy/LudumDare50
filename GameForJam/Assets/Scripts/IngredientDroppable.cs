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

        private Text tooltip;

        private SpriteRenderer image;
        
        private void Start()
        {
            tooltip = GetComponentInChildren<Text>();
            if (tooltip != null)
            {
                tooltip.text = GameManager.instance?.ingredientsBook?.Get(type)?.friendlyName ?? "not specified";
                tooltip.gameObject.SetActive(false);
            }

            image = GetComponentInChildren<SpriteRenderer>();
            if (tooltip!=null)
                image.sprite = GameManager.instance?.ingredientsBook?.Get(type)?.image;
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