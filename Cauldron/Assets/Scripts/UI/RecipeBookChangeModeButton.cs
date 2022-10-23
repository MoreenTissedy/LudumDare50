using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookChangeModeButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public RecipeBook.Mode mode;
        public Transform hint;
        private float hintAnimDuration = 0.3f;
        [Inject]
        private RecipeBook recipeBook;

        private float localScaleX;

        private void Awake()
        {
            var localScale = hint.localScale;
            localScaleX = localScale.x;
            localScale = new Vector3(0, localScale.y, localScale.z);
            hint.localScale = localScale;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            recipeBook.ChangeMode(mode);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hint.transform.DOScaleX(localScaleX, hintAnimDuration).From(0);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hint.transform.DOScaleX(0, hintAnimDuration);
        }
    }
}