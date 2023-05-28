using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CauldronCodebase
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler

    {
        public float sizeCoef = 1.2f;
        public float sizeSpeed = 0.2f;
        private bool clicked;
        private Vector3 initialScale;
        private RectTransform transf;
        private EndingScreen endingPanel;
        private RecipeBook recipeBook;

        [Inject]
        private void Construct(EndingScreen endingScreen, RecipeBook recipe)
        {
            endingPanel = endingScreen;
            recipeBook = recipe;
        }

        private void Start()
        {
            transf = GetComponent<RectTransform>();
            initialScale = transf.sizeDelta;
            //start flashing to attract attention
            transf.DOSizeDelta((initialScale * sizeCoef), sizeSpeed).SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            clicked = true;
            transf.DOKill(true);
            if (recipeBook.bookObject.enabled)
            {
                recipeBook.CloseBook();
            }
            else if (endingPanel.bookObject.enabled)
            {
                endingPanel.CloseBook();
            }
            else if (GameLoader.IsMenuOpen())
            {
                GameLoader.UnloadMenu();
            }
            else
            {
                GameLoader.LoadMenu();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //grow in size
            if (!clicked)
                transf.DOPause();
            transf.DOSizeDelta((initialScale * sizeCoef), sizeSpeed);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //shrink in size
            if (clicked)
                transf.DOSizeDelta((initialScale), sizeSpeed);
            else
            {
                transf.DOPlay();
            }
        }
    }
}