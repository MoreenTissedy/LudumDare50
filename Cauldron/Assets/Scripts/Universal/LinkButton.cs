using System.Collections;
using Cysharp.Threading.Tasks;
using CauldronCodebase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using System.Threading;

namespace Universal
{
    public class LinkButton: GrowOnMouseEnter
    {
        public string link = "https://vk.com/theironhearthg";
        [SerializeField] private ScrollTooltip tooltip;
        [SerializeField] private RectTransform icon;
        [SerializeField] private float delay = 2f;

        private bool isClicked = false;

        [Inject] private SoundManager sound;

        public void Start()
        {
            tooltip.Close();
            tooltip.SetText(link).Forget();
        }
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (isClicked) return;

            base.OnPointerClick(eventData);
            if (!string.IsNullOrEmpty(link))
            {
                Application.OpenURL(link);
                StartCoroutine(Animation());
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            sound.Play(Sounds.MenuFocus);

            if (isClicked) return;
            tooltip.Open();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            tooltip.Close();
        }

        private IEnumerator Animation()
        {
            isClicked = true;
            tooltip.Close();
            icon.DORotate(new Vector3(0, 360, 0), delay, RotateMode.LocalAxisAdd).SetUpdate(true);
            yield return new WaitForSecondsRealtime(delay);

            icon.DOKill();
            icon.localRotation = Quaternion.Euler(0, 0, 0);
            isClicked = false;
        }
    }
}