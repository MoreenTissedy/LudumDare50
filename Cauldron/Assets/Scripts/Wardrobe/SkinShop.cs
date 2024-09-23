using System.Collections.Generic;
using EasyLoc;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using Button = UnityEngine.UI.Button;

namespace CauldronCodebase
{
    public class SkinShop : Book
    {
        [Localize] [SerializeField] private string ownedText;
        [Localize] [SerializeField] private string cantBuyText = "Unlocked only by ending";
        [Localize] [SerializeField] private string priceText;
        
        [SerializeField] private string playersMoneyText;
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Color unavailableTextColor;

        [SerializeField] private SkeletonGraphic witchSkeleton;
        [SerializeField] private TMP_Text playersMoneyTMP;
        [SerializeField] private TMP_Text descriptionTMP;
        [SerializeField] private TMP_Text noPriceText;
        [SerializeField] private TMP_Text priceField;
        [SerializeField] private GameObject priceBlock;
        [SerializeField] private TMP_Text headerTMP;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private List<WardrobeCell> cells;

        [Inject] private SkinsProvider skinsProvider;
        [Inject] private GameDataHandler gameDataHandler;

        private int playersMoney;
        private WardrobeCell selectedCell;

        private string defaultColorHex, unavailableColorHex;

        private void Start()
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
            closeButton.onClick.AddListener(CloseBook);
            
            defaultColorHex = ColorUtility.ToHtmlStringRGB(defaultTextColor);
            unavailableColorHex = ColorUtility.ToHtmlStringRGB(unavailableTextColor);
        }

        protected override void InitTotalPages()
        {
            totalPages = 1;
        }
        
        protected override void UpdatePage()
        {
            playersMoney = gameDataHandler.Money;
            
            for (int i = 0; i < skinsProvider.skins.Length; i++)
            {
                var skin = skinsProvider.skins[i];
                cells[i].Setup(skin, DetermineCellState(skin));
                cells[i].OnClick += OnCellClicked;
            }
        }

        private WardrobeCellState DetermineCellState(SkinSO skin)
        {
            if (skinsProvider.Unlocked(skin.name))
            {
                return WardrobeCellState.Owned;
            }
            if (skin.Price <= 0 || skin.Price > playersMoney)
            {
                return WardrobeCellState.Unavailable;
            }
            return WardrobeCellState.Available;
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
            witchSkeleton.Skeleton.SetSkin(skin.name);
            witchSkeleton.AnimationState.AddAnimation(0, "Idle", true, 0);
            descriptionTMP.text = skin.DescriptionText;
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
            priceField.text = text;
            buyButton.interactable = false;
        }

        private void SetPrice(int price)
        {
            priceBlock.SetActive(true);
            if (price >= playersMoney)
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

                UpdatePLayerMoneyUI();
                selectedCell.SetState(WardrobeCellState.NewlyUnlocked);
                buyButton.interactable = false;
            }
        }

        private void UpdatePLayerMoneyUI()
        {
            playersMoneyTMP.text = $"{playersMoneyText}: <color=#{defaultColorHex}>{playersMoney}</color>";
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
