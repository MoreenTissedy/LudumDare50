using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class PremiumSkinButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sprite activeButton;
        [SerializeField] private Sprite notActiveButton;
        [SerializeField] private GameObject cauldronStandart;
        [SerializeField] private GameObject cauldronPremium;
        [SerializeField] private SkinSO skinPremium;

        private Image image;

        [Inject] private GameDataHandler gameData;
        [Inject] private Cauldron cauldron;
        [Inject] private WitchSkinChanger witch;
        
        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void Start()
        {
            UpdateSkins();
        }
        
        private void UpdateSkins()
        {
            UpdateButton();
            UpdateCauldron();
            UpdateWitch();
        }

        private void UpdateButton()
        {
            image.sprite = gameData.premiumSkin ? activeButton : notActiveButton;
        }
        
        private void UpdateCauldron()
        {
            cauldron.ChangeVisual(gameData.premiumSkin ? cauldronPremium : cauldronStandart);
        }
        
        private void UpdateWitch()
        {
            witch.ChangeSkin(gameData.premiumSkin ? skinPremium : gameData.currentSkin);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            gameData.premiumSkin = !gameData.premiumSkin;
            UpdateSkins();
        }
    }
}