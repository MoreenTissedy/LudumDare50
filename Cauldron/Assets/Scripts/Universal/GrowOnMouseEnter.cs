using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Universal
{
    [RequireComponent(typeof(RectTransform))]
    public class GrowOnMouseEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public float sizeCoef = 1.2f;
        public float sizeSpeed = 0.2f;
        
        private Vector3 initialScale;
        private RectTransform transf;

        private void Awake()
        {
            transf = GetComponent<RectTransform>();
            initialScale = transf.localScale;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            transf.DOScale((initialScale * sizeCoef), sizeSpeed).SetUpdate(true);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            transf.DOScale((initialScale), sizeSpeed).SetUpdate(true);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            transf.DOKill();
            transf.localScale = initialScale;
        }
    }
}