using System.Collections;
using UnityEngine;

namespace CauldronCodebase
{
    public class Tutorial : MonoBehaviour
    {
        public RectTransform[] pages;
        public Canvas canvas;
        private void Start()
        {
            CloseAllPages();
        }

        public void CloseAllPages()
        {
            canvas.enabled = false;
            StopAllCoroutines();
            foreach (var page in pages)
            {
                page.gameObject.SetActive(false);
            }
        }

        private void Show(int page)
        {
            canvas.enabled = true;
            if (page>=pages.Length)
                return;
            pages[page].gameObject.SetActive(true);
        }

        public void ShowGameplay()
        {
            StartCoroutine(GameplayTutorialCoroutine());
        }

        public void ShowOnBook()
        {
            Show(1);
            StartCoroutine(WaitForInput());
        }

        IEnumerator GameplayTutorialCoroutine()
        {
            Show(0);
            yield return new WaitForSeconds(3);
            yield return new WaitUntil(() => Input.anyKeyDown);
            Show(2);
            yield return new WaitForSeconds(3);
            yield return new WaitUntil(() => Input.anyKeyDown);
            CloseAllPages();
        }
        
        IEnumerator WaitForInput()
        {
            yield return new WaitForSeconds(3);
            yield return new WaitUntil(() => Input.anyKeyDown);
            CloseAllPages();
        }
    }
}