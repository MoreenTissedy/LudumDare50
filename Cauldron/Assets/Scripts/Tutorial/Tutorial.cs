using System.Collections;
using UnityEngine;

namespace CauldronCodebase
{
    public class Tutorial : MonoBehaviour
    {
        public RectTransform[] pages;

        private int helpAsked = 0;
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
            StartCoroutine(WaitForInput());
        }

        public void ShowGameplay()
        {
            Debug.Log("show gameplay");
            //show 1 and 2 interchangeably
            if (helpAsked%2 == 1)
                Show(2);
            else if (helpAsked%2 == 0)
                Show(0);
            helpAsked++;
        }

        public void ShowOnBook()
        {
            Debug.Log("show tutorial book");
            Show(1);
        }

        IEnumerator WaitForInput()
        {
            yield return new WaitUntil(() => Input.anyKeyDown);
            CloseAllPages();
        }
    }
}