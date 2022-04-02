using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class IngredientDroppable: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Ingredients type;

        private Text tooltip;
        
        private void Start()
        {
            tooltip = GetComponentInChildren<Text>();
            if (tooltip != null)
            {
                tooltip.text = GameManager.instance?.ingredientsBook?.Get(type)?.friendlyName ?? "not specified";
                tooltip.gameObject.SetActive(false);
            }

            var image = GetComponentInChildren<SpriteRenderer>();
            if (tooltip!=null)
                image.sprite = GameManager.instance?.ingredientsBook?.Get(type)?.image;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Cauldron.instance.AddToMix(type);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip is null)
                return;
            tooltip.gameObject.SetActive(false);
        }
    }
}