using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class EndingScreen : Book
    {
        [Inject]
        private EndingsProvider endings;
        
        public Text text;
        public Image image;

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