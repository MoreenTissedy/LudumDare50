using System.Collections;
using UnityEngine;

namespace CauldronCodebase
{
    public class Tutorial : MonoBehaviour
    {
        public RectTransform[] pages;
        private void Start()
        {
            CloseAllPages();
        }

        private void CloseAllPages()
        {
            foreach (var page in pages)
            {
                page.gameObject.SetActive(false);
            }
        }

        private void Show(int page)
        {
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
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => Input.anyKeyDown);
            Show(2);
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => Input.anyKeyDown);
            CloseAllPages();
        }
        
        IEnumerator WaitForInput()
        {
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => Input.anyKeyDown);
            CloseAllPages();
        }
    }
}