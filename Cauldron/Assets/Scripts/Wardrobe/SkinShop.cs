using System;
using System.Collections.Generic;
using System.Linq;
using EasyLoc;
using Spine.Unity;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace CauldronCodebase
{
    public class SkinShop : Book
    {
        [Localize] [SerializeField] private string ownedText;
        [Localize] [SerializeField] private string cantBuyText = "Unlocked only by ending";
        
        [SerializeField] private string playersMoneyText;
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Color unavailableTextColor;

        [SerializeField] private SkeletonGraphic witchSkeleton;
        [SerializeField] private TMP_Text playersMoneyTMP;
        [SerializeField] private TMP_Text descriptionTMP;
        [SerializeField] private TMP_Text description2TMP;
        [SerializeField] private TMP_Text noPriceText;
        [SerializeField] private TMP_Text priceField;
        [SerializeField] private GameObject priceBlock;
        [SerializeField] private TMP_Text headerTMP;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private List<WardrobeCell> cells;

        [SerializeField] private SkinsProvider skinsProvider;

        private int playersMoney;
        private SkinSO initialSkin;
        private bool initialSkinJustUnlocked;
        private WardrobeCell selectedCell;

        private string defaultColorHex, unavailableColorHex;

        private void Start()
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
            closeButton.onClick.AddListener(CloseBook);
            
            defaultColorHex = ColorUtility.ToHtmlStringRGB(defaultTextColor);
            unavailableColorHex = ColorUtility.ToHtmlStringRGB(unavailableTextColor);
        }

        public void SetPlayerMoney(int playerMoney)
        {
            playersMoney = playerMoney;
        }
        
        public void SetInitialSkin(SkinSO skin, bool unlock)
        {
            initialSkin = skin;
            initialSkinJustUnlocked = unlock;
            if (unlock)
            {
                skinsProvider.TryUnlock(skin);
            }
        }

        public bool CanBeOpened(int playerMoney)
        {
            return skinsProvider.GetUnlockedSkinsCount() > 1 || playerMoney >= skinsProvider.GetMinimumPrice();
        }

        protected override void InitTotalPages()
        {
            totalPages = 1;
        }
        
        protected override void UpdatePage()
        {
            var skins = skinsProvider.skins;
            List<(WardrobeCellState, SkinSO)> sortedList = new List<(WardrobeCellState, SkinSO)>();
            foreach (var skinSo in skins)
            {
                sortedList.Add((DetermineCellState(skinSo), skinSo));
            }

            int i = 0;
            foreach (var (wardrobeCellState, skinSo) in sortedList.OrderBy(x => x.Item1))
            {
                cells[i].Setup(skinSo, wardrobeCellState);
                cells[i].OnClick += OnCellClicked;
                if (skinSo == initialSkin)
                {
                    selectedCell = cells[i];
                    selectedCell.ToggleSelect(true);
                }
                i++;
            }
            
            UpdateSelectedSkinUI(initialSkin);
            UpdatePlayerMoneyUI();
        }

        private WardrobeCellState DetermineCellState(SkinSO skin)
        {
            if (skin == initialSkin && initialSkinJustUnlocked)
            {
                return WardrobeCellState.NewlyUnlocked;
            }
            if (skinsProvider.Unlocked(skin.name))
            {
                return WardrobeCellState.Owned;
            }
            if (skin.Price <= 0 || skin.Price > playersMoney)
            {
                return WardrobeCellState.UnavailableToBuy;
            }
            return WardrobeCellState.AvailableToBuy;
        }

        private void OnCellClicked(WardrobeCell cell)
        {
            if (cell == selectedCell) return;

            if (selectedCell != null)
            {
                selectedCell.ToggleSelect(false);
            }
            selectedCell = cell;
            cell.ToggleSelect(true);
            
            UpdateSelectedSkinUI(cell.Skin);
        }

        private void UpdateSelectedSkinUI(SkinSO skin)
        {
            witchSkeleton.AnimationState.SetAnimation(0, "Active", false);
            witchSkeleton.Skeleton.SetSkin(skin.SpineName);
            witchSkeleton.AnimationState.AddAnimation(0, "Idle", true, 0);
            descriptionTMP.text = skin.FlavorText;
            description2TMP.text = skin.DescriptionText;
            headerTMP.text = skin.FriendlyName;
            
            buyButton.gameObject.SetActive(!skinsProvider.Unlocked(skin.name));

            if (skinsProvider.Unlocked(skin.name))
            {
                SetPrice(ownedText);
            }
            else if (skin.Price <= 0)
            {
                SetPrice(cantBuyText);
            }
            else
            {
                SetPrice(skin.Price);
            }
        }

        private void SetPrice(string text)
        {
            priceBlock.SetActive(false);
            noPriceText.gameObject.SetActive(true);
            noPriceText.text = text;
            buyButton.gameObject.SetActive(false);
        }

        private void SetPrice(int price)
        {
            priceBlock.SetActive(true);
            buyButton.gameObject.SetActive(true);
            if (price > playersMoney)
            {
                priceField.text = $"<color=#{unavailableColorHex}>{price}</color>";
                buyButton.interactable = false;
            }
            else
            {
                priceField.text = $"<color=#{defaultColorHex}>{price}</color>";
                buyButton.interactable = true;
            }
            priceField.text = price.ToString();
            noPriceText.gameObject.SetActive(false);
        }

        private void OnBuyButtonClicked()
        {
            if (selectedCell != null && playersMoney >= selectedCell.Skin.Price && !skinsProvider.Unlocked(selectedCell.Skin.name))
            {
                playersMoney -= selectedCell.Skin.Price;
                
                skinsProvider.TryUnlock(selectedCell.Skin);

                UpdatePlayerMoneyUI();
                selectedCell.SetState(WardrobeCellState.NewlyUnlocked);
                SetPrice(ownedText);
                
                foreach (var cell in cells)
                {
                    if (selectedCell == cell)
                    {
                        continue;
                    }
                    cell.SetState(DetermineCellState(cell.Skin));
                }
            }
        }

        private void UpdatePlayerMoneyUI()
        {
            playersMoneyTMP.text = $"<color=#{defaultColorHex}>{playersMoney}</color>";
        }

        public override void CloseBook()
        {
            foreach (var cell in cells)
            {
                cell.OnClick -= OnCellClicked;
            }
            
            base.CloseBook();
        }
    }
}
