using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace CauldronCodebase
{
    public abstract class Book : MonoBehaviour
    {
        [SerializeField] public GameObject bookObject;
        [SerializeField] protected bool keyboardControl;
        [FormerlySerializedAs("rightCorner")] [SerializeField] protected GameObject nextPageButton;
        [FormerlySerializedAs("leftCorner")] [SerializeField] protected GameObject prevPageButton;
        [FormerlySerializedAs("left")] [SerializeField] protected AudioSource leftPageSound;
        [FormerlySerializedAs("right")] [SerializeField] protected AudioSource rightPageSound;
        [SerializeField] protected AudioClip openCloseSound;

        protected int currentPage = 0;
        protected int totalPages = 3;

        void OnValidate()
        {
            if (bookObject is null)
            {
                bookObject = gameObject;
            }
        }

        protected virtual void Awake()
        {
            CloseBook();
            prevPageButton.SetActive(false);
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
            if (bookObject.activeInHierarchy)
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
            leftPageSound.PlayOneShot(openCloseSound);
            rightPageSound.PlayOneShot(openCloseSound);
            bookObject.SetActive(true);
            StartCoroutine(UpdateWithDelay());
        }
        
        IEnumerator UpdateWithDelay()
        {
            yield return null;
            UpdatePage();
        }
        
        public virtual void CloseBook()
        {
            leftPageSound.PlayOneShot(openCloseSound);
            rightPageSound.PlayOneShot(openCloseSound);
            bookObject.SetActive(false);
        }
        
        public void NextPage()
        {
            if (!bookObject.activeInHierarchy) return;
            if (currentPage + 1 >= totalPages) return;
            currentPage++;
            UpdateBookButtons();
            UpdatePage();
            rightPageSound.Play();
        }

        public void PrevPage()
        {
            if (!bookObject.activeInHierarchy) return;
            if (currentPage <= 0) return;
            currentPage--;
            UpdateBookButtons();
            UpdatePage();
            leftPageSound.Play();
        }

        public void OpenPage(int i)
        {
            if (i < 0 || i >= totalPages) return;
            if (i == currentPage) return;
            //sound
            if (i < currentPage)
            {
                leftPageSound.Play();   
            }
            else if (i > currentPage)
            {
                rightPageSound.Play();
            }

            currentPage = i;
            UpdateBookButtons();
            UpdatePage();
        }

        private void UpdateBookButtons()
        {
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