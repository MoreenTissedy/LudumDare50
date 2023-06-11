using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Universal
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject tooltip;
        [SerializeField] private float fadeDuration = 0.3f;

        private CanvasGroup canvasGroup;

        private void Start()
        {
            tooltip.SetActive(false);
            if (!tooltip.TryGetComponent(out canvasGroup))
            {
                canvasGroup = tooltip.AddComponent<CanvasGroup>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltip.gameObject.SetActive(true);
            canvasGroup.DOFade(1, fadeDuration).From(0);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            canvasGroup.DOFade(0, fadeDuration).OnComplete(() => tooltip.gameObject.SetActive(false));
        }

        private void OnDestroy()
        {
            canvasGroup.DOKill();
        }
    }
}