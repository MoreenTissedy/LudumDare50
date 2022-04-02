using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class IngredientDroppable: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Ingredients type;
        public GameObject tooltip;

        private void Awake()
        {
            tooltip.GetComponentInChildren<Text>().text = GameManager.instance.ingredientsBook.Get(type).friendlyName;
            var image = GetComponentInChildren<SpriteRenderer>();
            image.sprite = GameManager.instance.ingredientsBook.Get(type).image;
            tooltip.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Cauldron.instance.AddToMix(type);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip.SetActive(false);
        }
    }
}