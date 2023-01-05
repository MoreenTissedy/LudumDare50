using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace CauldronCodebase
{
    public abstract class Book : MonoBehaviour
    {
        [SerializeField] public Canvas bookObject;
        [SerializeField] protected RectTransform mainPanel;
        [SerializeField] protected bool keyboardControl;
        [SerializeField] protected bool buttonControl;
        [FormerlySerializedAs("rightCorner")] [SerializeField] protected GameObject nextPageButton;
        [FormerlySerializedAs("leftCorner")] [SerializeField] protected GameObject prevPageButton;
        [FormerlySerializedAs("left")] [SerializeField] protected AudioSource leftPageSound;
        [FormerlySerializedAs("right")] [SerializeField] protected AudioSource rightPageSound;
        [SerializeField] protected AudioClip openCloseSound;
        [SerializeField] private float openCloseAnimationTime = 0.5f;

        private float offScreenYPos, initialYPos;
        protected int currentPage = 0;
        protected int totalPages = 3;
        public int CurrentPage => currentPage;
        public int TotalPages => totalPages;
        
        public event Action OnClose;
/*
        void OnValidate()
        {
            if (bookObject == null)
            {
                bookObject;
            }
        }
*/
        protected virtual void Awake()
        {
            //cache initial position
            initialYPos = mainPanel.anchoredPosition.y;
            //cache offscreen position
            offScreenYPos = 1080+initialYPos;
            
            bookObject.enabled = false;
            UpdateBookButtons();
        }

        protected virtual void Update()
        {
            if (!keyboardControl)
                return;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                NextPage();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                PrevPage();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseBook();
            }
        }
        
        protected abstract void InitTotalPages();
        protected abstract void UpdatePage();
        
        public void ToggleBook()
        {
            if (bookObject.enabled)
            {
                CloseBook();
            }
            else
            {
                OpenBook();
            }
        }

        public virtual void OpenBook()
        {
            PlayOpenCloseSound();
            bookObject.enabled = true;
            mainPanel.DOLocalMoveY(initialYPos, openCloseAnimationTime).
                From(offScreenYPos);
            StartCoroutine(UpdateWithDelay());
            UpdateBookButtons();
        }

        private void PlayOpenCloseSound()
        {
            if (openCloseSound)
            {
                leftPageSound?.PlayOneShot(openCloseSound);
                rightPageSound?.PlayOneShot(openCloseSound);
            }
        }

        IEnumerator UpdateWithDelay()
        {
            yield return null;
            UpdatePage();
        }
        
        public virtual void CloseBook()
        {
            PlayOpenCloseSound();
            OnClose?.Invoke();
            mainPanel.DOLocalMoveY(offScreenYPos, openCloseAnimationTime).
                OnComplete(() =>
                {
                    bookObject.enabled = false;
                });
        }
        
        public virtual void NextPage()
        {
            if (!bookObject.enabled) return;
            if (currentPage + 1 >= totalPages) return;
            currentPage++;
            UpdateBookButtons();
            UpdatePage();
            if (rightPageSound) rightPageSound.Play();
        }

        public virtual void PrevPage()
        {
            if (!bookObject.enabled) return;
            if (currentPage <= 0) return;
            currentPage--;
            UpdateBookButtons();
            UpdatePage();
            if (leftPageSound) leftPageSound.Play();
        }

        public void OpenPage(int i)
        {
            if (i < 0 || i >= totalPages) return;
            if (i == currentPage) return;
            //sound
            if (i < currentPage)
            {
                leftPageSound?.Play();   
            }
            else if (i > currentPage)
            {
                rightPageSound?.Play();
            }

            currentPage = i;
            UpdateBookButtons();
            UpdatePage();
        }

        protected virtual void UpdateBookButtons()
        {
            if (!buttonControl || prevPageButton is null || nextPageButton is null)
                return;
            prevPageButton.SetActive(true);
            nextPageButton.SetActive(true);
            if (currentPage == 0)
            {
                prevPageButton.SetActive(false);
            }
            else if (currentPage + 1 == totalPages)
            {
                nextPageButton.SetActive(false);
            }
        }
    }
}