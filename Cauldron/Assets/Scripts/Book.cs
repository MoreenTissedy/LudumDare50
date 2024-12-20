using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace CauldronCodebase
{
    public abstract class Book : MonoBehaviour
    {
        [SerializeField] public Canvas bookObject;
        [SerializeField] protected RectTransform mainPanel;
        [SerializeField] protected bool keyboardControl;
        [SerializeField] protected bool buttonControl;
        [SerializeField] protected GameObject nextPageButton;
        [SerializeField] protected GameObject prevPageButton;
        [SerializeField] protected BookSounds sounds;
        [SerializeField] private float openCloseAnimationTime = 0.5f;

        private float offScreenYPos, initialYPos;
        protected int currentPage = 0;
        protected int totalPages = 3;
        public int CurrentPage => currentPage;
        public int TotalPages => totalPages;

        public bool IsOpen { get; private set; }

        protected SoundManager SoundManager;
        private Controls controls;
        
        public event Action OnClose;

        [Inject]
        protected virtual void ConstructBase(SoundManager soundManager, InputManager inputManager)
        {
            SoundManager = soundManager;
            controls = inputManager.Controls;
        }
        
        protected virtual void Awake()
        {
            //cache initial position
            initialYPos = mainPanel.anchoredPosition.y;
            //cache offscreen position
            offScreenYPos = 1080+initialYPos;
            
            bookObject.enabled = false;
            UpdateBookButtons();

            if (keyboardControl)
            {
                controls.General.BookNavigate.performed += ProcessNavigate;
            }
        }

        private void ProcessNavigate(InputAction.CallbackContext input)
        {
            var leftRight = input.ReadValue<Vector2>().x;
            if (leftRight > 0)
            {
                NextPage();
            }
            else if (leftRight < 0)
            {
                PrevPage();
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
            SoundManager.PlayBook(sounds, BookSound.Open);
            bookObject.enabled = true;
            IsOpen = true;
            mainPanel.DOLocalMoveY(initialYPos, openCloseAnimationTime).
                From(offScreenYPos);
            StartCoroutine(UpdateWithDelay());
            UpdateBookButtons();
        }

        IEnumerator UpdateWithDelay()
        {
            yield return null;
            UpdatePage();
        }
        
        public virtual void CloseBook()
        {
            SoundManager.PlayBook(sounds, BookSound.Close);
            OnClose?.Invoke();
            IsOpen = false;
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
            SoundManager.PlayBook(sounds, BookSound.Right);
        }

        public virtual void PrevPage()
        {
            if (!bookObject.enabled) return;
            if (currentPage <= 0) return;
            currentPage--;
            UpdateBookButtons();
            UpdatePage();
            SoundManager.PlayBook(sounds, BookSound.Left);
        }

        public void OpenPage(int i)
        {
            if (i < 0 || i >= totalPages) return;
            //if (i == currentPage) return;
            if (i < currentPage)
            {
                SoundManager.PlayBook(sounds, BookSound.Left);  
            }
            else if (i > currentPage)
            {
                SoundManager.PlayBook(sounds, BookSound.Right);
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