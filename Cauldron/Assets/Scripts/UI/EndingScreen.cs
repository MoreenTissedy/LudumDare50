using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

namespace CauldronCodebase
{
    public class EndingScreen : Book
    {
        [Inject]
        private EndingsProvider endings;

        [Header("Ending display")] 
        public TMP_Text title;
        public TMP_Text text;
        public Image image;
        public event Action<int> OnPageUpdate;
        
        
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
        }

        public void OpenBookWithEnding(Ending ending)
        {
            currentPage = endings.GetIndexOf(ending);
            OpenBook();
            if (!endings.unlocked[currentPage])
            {
                UnlockThisEnding();
            }
        }

        private void UnlockThisEnding()
        {
            endings.unlocked[currentPage] = true;
            image.DOFade(1, 3f);
            DOTween.To(() => text.alpha, (i) => text.alpha = i, 1, 3f);
        }

        private void Show(Ending ending)
        {
            title.text = ending.title;
            image.sprite = ending.image;
            text.text = ending.text;
            if (endings.unlocked[endings.GetIndexOf(ending)])
            {
                text.alpha = 1;
                image.color = Color.white;
            }
            else
            {
                text.alpha = 0;
                image.color = Color.clear;
            }
        }
        

        protected override void InitTotalPages()
        {
            totalPages = endings.endings.Length;
        }

        protected override void UpdatePage()
        {
            Show(endings.endings[currentPage]);
            OnPageUpdate?.Invoke(currentPage);
        }

        protected override void UpdateBookButtons()
        {
            //cyclic pages - do nothing
        }

        public override void NextPage()
        {
            if (currentPage == totalPages - 1)
            {
                currentPage = -1;
            }
            base.NextPage();
        }

        public override void PrevPage()
        {
            if (currentPage == 0)
            {
                currentPage = totalPages;
            }
            base.PrevPage();
        }
    }
}