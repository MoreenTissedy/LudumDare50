using System.Collections;
using Cysharp.Threading.Tasks;
using CauldronCodebase;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Universal
{
    public class LinkButton: AnimatedButtonComponent
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

        private IEnumerator AnimationAndOpenLink()
        {
            isClicked = true;
            tooltip.Close();
            icon.DORotate(new Vector3(0, 360, 0), delay, RotateMode.LocalAxisAdd).SetUpdate(true);
            yield return new WaitForSecondsRealtime(delay * 0.25f);

            if (!string.IsNullOrEmpty(link))
            {
                Application.OpenURL(link);
            }          
            yield return new WaitForSecondsRealtime(delay * 0.75f);

            icon.DOKill();
            icon.localRotation = Quaternion.Euler(0, 0, 0);
            isClicked = false;
        }

        public override void Select()
        {
            sound.Play(Sounds.MenuFocus);

            if (isClicked) return;
            tooltip.Open();
        }

        public override void Unselect()
        {
            tooltip.Close();
        }

        public override void Activate()
        {
            if (isClicked) return;

            StartCoroutine(AnimationAndOpenLink());
        }

        public override void ChangeInteractive(bool isInteractive){}
    }
}