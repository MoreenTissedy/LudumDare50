using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class EndingScreen : Book
    {
        [Inject]
        private EndingsProvider endings;

        [Header("Ending display")]
        public TMP_Text text;
        public Image image;

        //DEBUG - no menu
        protected override void Update()
        {
            base.Update();
            if (!bookObject.activeInHierarchy && Input.GetKeyDown(KeyCode.K))
            {
                OpenBook();
            }
        }

        public override void OpenBook()
        {
            base.OpenBook();
            InitTotalPages();
            currentPage = 0;
        }

        public void OpenBookOnPage(Ending ending)
        {
            OpenBook();
            currentPage = endings.GetIndexOf(ending);
        }

        private void Show(Ending ending)
        {
            text.text = ending.text;
            image.sprite = ending.image;
        }
        

        protected override void InitTotalPages()
        {
            totalPages = endings.endings.Length;
        }

        protected override void UpdatePage()
        {
            Show(endings.endings[currentPage]);
        }
    }
}