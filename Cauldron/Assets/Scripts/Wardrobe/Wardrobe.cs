using System.Collections.Generic;
using System.Linq;
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
        
        [SerializeField] private SkinSO[] skinModels;
        [SerializeField] private WardrobeCell[] wardrobeCells;

        [Header("Animation")]
        [SerializeField] private float descriptionPanelShowSpeed;
        [SerializeField] private float descriptionPanelXPos;

        [Inject] private WitchSkinChanger witchSkinChanger;

        private WardrobeCell _selectedCell;
        private float _initDescriptionPanelPos;

        private void Start()
        {
            closeButton.onClick.AddListener(CloseBook);
            confirmSkinButton.onClick.AddListener(CloseBook);
            _initDescriptionPanelPos = descriptionPanel.anchoredPosition.x;
        }

        protected override void InitTotalPages()
        {
            totalPages = 1;
        }
        
        protected override void UpdatePage()
        {
            var unlockedSkins = GetUnlockedSkins();
            for (int i = 0; i < wardrobeCells.Length; i++)
            {
                skinModels[i].IsAvailable = unlockedSkins.Contains(skinModels[i].SkinName);
                wardrobeCells[i].Setup(skinModels[i]);
                wardrobeCells[i].OnClick += SelectCell;
                if (witchSkinChanger.CurrentSkin == skinModels[i])
                {
                    _selectedCell = wardrobeCells[i];
                    _selectedCell.ToggleSelect(true);
                }
                else
                {
                    wardrobeCells[i].ToggleSelect(false);
                }
            }
        }

        private void SelectCell(WardrobeCell cell)
        {
            if (cell == _selectedCell) return;
            
            _selectedCell.ToggleSelect(false);
            _selectedCell = cell;
            cell.ToggleSelect(true);

            description.text = cell.Skin.Text;
            ShowDescriptionPanel();
            ConfirmSkin();
        }

        private void ShowDescriptionPanel()
        {
            descriptionPanel.DOLocalMoveX(descriptionPanelXPos, descriptionPanelShowSpeed).SetEase(Ease.OutQuart);
        }

        private void HideDescriptionPanel()
        {
            descriptionPanel.DOLocalMoveX(_initDescriptionPanelPos, descriptionPanelShowSpeed).SetEase(Ease.OutQuart);
        }

        private void ConfirmSkin()
        {
            if (!witchSkinChanger.SkinChangeAvailable) return;
            
            witchSkinChanger.ChangeSkin(_selectedCell.Skin);
            //HideDescriptionPanel();
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
        
        private List<string> GetUnlockedSkins()
        {
            string[] unlockedEndings = PlayerPrefs.GetString(PrefKeys.UnlockedEndings).Split(',');
            List<string> skinList = new List<string>() {"main"};
            foreach (var skin in skinModels)
            {
                if (skin.LastUnlockedEnding.Any(ending => unlockedEndings.Contains(ending)) && !skinList.Contains(skin.SkinName))
                {
                    skinList.Add(skin.SkinName);
                }
            }

            return skinList;
        }
    }
}