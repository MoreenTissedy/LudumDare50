﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class Wardrobe : Book
    {
        [SerializeField] private RectTransform descriptionPanel;
        [SerializeField] private TMP_Text description;
        
        [SerializeField] private Button confirmSkinButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private WardrobeCell[] wardrobeCells;

        [Header("Animation")]
        [SerializeField] private float descriptionPanelShowSpeed;
        [SerializeField] private float descriptionPanelHiddenYPos;

        [Inject] private WitchSkinChanger witchSkinChanger;
        [Inject] private SkinsProvider skinsProvider;

        private WardrobeCell selectedCell;
        private float initDescriptionPanelPos;

        public event Action<SkinSO> SkinApplied;

        private void Start()
        {
            closeButton.onClick.AddListener(CloseBook);
            confirmSkinButton.onClick.AddListener(ApplySkin);
            InitDescriptionPanelAnimation();
        }

        private void InitDescriptionPanelAnimation()
        {
            var anchoredPosition = descriptionPanel.anchoredPosition;
            initDescriptionPanelPos = anchoredPosition.y;
            anchoredPosition = new Vector2(anchoredPosition.x, descriptionPanelHiddenYPos);
            descriptionPanel.anchoredPosition = anchoredPosition;
        }

        protected override void InitTotalPages()
        {
            totalPages = 1;
        }
        
        protected override void UpdatePage()
        {
            var skinsProviderSkins = skinsProvider.skins;
            LinkedList<SkinSO> sortedSkins = new LinkedList<SkinSO>();
            foreach (SkinSO skin in skinsProviderSkins)
            {
                if (skinsProvider.Unlocked(skin.name))
                {
                    sortedSkins.AddFirst(skin);
                }
                else
                {
                    sortedSkins.AddLast(skin);
                }
            }
            for (int i = 0; i < sortedSkins.Count; i++)
            {
                var skin = skinsProviderSkins[i];
                wardrobeCells[i].Setup(skin, DetermineCellState(skin));
                wardrobeCells[i].OnClick += SelectCell;
                if (witchSkinChanger.CurrentSkin == skin)
                {
                    selectedCell = wardrobeCells[i];
                    selectedCell.ToggleSelect(true);
                }
                else
                {
                    wardrobeCells[i].ToggleSelect(false);
                }
            }
        }

        private WardrobeCellState DetermineCellState(SkinSO skin)
        {
            //TODO: add display of the newly opened skin
            if (skinsProvider.Unlocked(skin.name))
            {
                return WardrobeCellState.Available;
            }

            return WardrobeCellState.Unavailable;
        }
        
        private void SelectCell(WardrobeCell cell)
        {
            if (cell == selectedCell) return;
            
            selectedCell.ToggleSelect(false);
            selectedCell = cell;
            cell.ToggleSelect(true);

            description.text = cell.Skin.DescriptionText;
            ShowDescriptionPanel();
            ConfirmSkin();
        }

        private void ShowDescriptionPanel()
        {
            descriptionPanel.DOLocalMoveY(initDescriptionPanelPos, descriptionPanelShowSpeed).SetEase(Ease.OutQuart);
        }

        private void HideDescriptionPanel()
        {
            descriptionPanel.DOLocalMoveY(descriptionPanelHiddenYPos, descriptionPanelShowSpeed).SetEase(Ease.OutQuart);
        }

        private void ConfirmSkin()
        {
            if (!witchSkinChanger.SkinChangeAvailable) return;
            
            witchSkinChanger.ChangeSkin(selectedCell.Skin);
            //HideDescriptionPanel();
        }

        public void ApplySkin()
        {
            //save skin as applied
            //confirm message ? 
            SkinApplied?.Invoke(selectedCell.Skin);
            CloseBook();
            //disable wardrobe
        }

        public override void CloseBook()
        {
            HideDescriptionPanel();
            foreach (var cell in wardrobeCells)
            {
                cell.OnClick -= SelectCell;
            }
            
            base.CloseBook();
        }
    }
}