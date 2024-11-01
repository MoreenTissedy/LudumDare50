using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class Wardrobe : Book
    {
        [SerializeField] private RectTransform descriptionPanel;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text description2;
        
        [SerializeField] private Button confirmSkinButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private WardrobeCell[] wardrobeCells;

        [Header("Animation")]
        [SerializeField] private float descriptionPanelShowSpeed;
        [SerializeField] private float descriptionPanelHiddenYPos;
        
        [Header("Confirmation")]
        [SerializeField] private ScrollTooltip tooltipPrefab;
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button acceptButton;

        [Inject] private WitchSkinChanger witchSkinChanger;
        [Inject] private SkinsProvider skinsProvider;
        [Inject] private GameDataHandler gameDataHandler;
        [Inject] private IAchievementManager achievements;

        private WardrobeCell selectedCell;
        private float initDescriptionPanelPos;

        private Action OnClosed;

        public Func<UniTask<bool>> OnApplyCondition;

        public event Action<SkinSO> SkinApplied;

        private void Start()
        {
            closeButton.onClick.AddListener(CloseWithoutApply);
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

            int i = 0;
            foreach (SkinSO skin in sortedSkins)
            {
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

                i++;
            }
        }

        private WardrobeCellState DetermineCellState(SkinSO skin)
        {
            //TODO: add display of the newly opened skin ?
            if (skinsProvider.Unlocked(skin.name))
            {
                return WardrobeCellState.None;
            }

            return WardrobeCellState.Locked;
        }
        
        private void SelectCell(WardrobeCell cell)
        {
            if (cell == selectedCell) return;
            
            selectedCell.ToggleSelect(false);
            selectedCell = cell;
            cell.ToggleSelect(true);

            description.text = cell.Skin.DescriptionText;
            description2.text = cell.Skin.FlavorText;
            ShowDescriptionPanel();
            TrySkin();
        }

        private void ShowDescriptionPanel()
        {
            descriptionPanel.DOLocalMoveY(initDescriptionPanelPos, descriptionPanelShowSpeed).SetEase(Ease.OutQuart);
        }

        private void HideDescriptionPanel()
        {
            descriptionPanel.DOLocalMoveY(descriptionPanelHiddenYPos, descriptionPanelShowSpeed).SetEase(Ease.OutQuart);
        }

        private void TrySkin()
        {
            achievements.TryUnlock("skin "+selectedCell.Skin.name.ToLowerInvariant());
            if (!witchSkinChanger.SkinChangeAvailable) return;
            
            witchSkinChanger.ChangeSkin(selectedCell.Skin);
        }

        public async void ApplySkin()
        {
            if (selectedCell.Skin.NeedsApprove && 
                !await tooltipPrefab.ShowAsDialog(selectedCell.Skin.ApproveMessage, acceptButton, rejectButton))
            {
                return;
            }
            if (OnApplyCondition != null && !await OnApplyCondition.Invoke())
            {
                return;
            }

            gameDataHandler.currentSkin = selectedCell.Skin;
            SkinApplied?.Invoke(selectedCell.Skin);
            CloseBook();
        }

        public void CloseWithoutApply()
        {
            witchSkinChanger.ChangeSkin(gameDataHandler.currentSkin);
            CloseBook();
        }

        public override void CloseBook()
        {
            HideDescriptionPanel();
            foreach (var cell in wardrobeCells)
            {
                cell.OnClick -= SelectCell;
            }
            
            base.CloseBook();
            OnClosed?.Invoke();
        }

        public void OpenWithCallback(Action onClosed)
        {
            OnClosed = onClosed;
            OpenBook();
        }
    }
}