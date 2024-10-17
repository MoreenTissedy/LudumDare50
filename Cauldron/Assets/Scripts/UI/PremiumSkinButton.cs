using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class PremiumSkinButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private float animationDelay = 0.15f;
        [SerializeField] private Sprite activeButton;
        [SerializeField] private Sprite notActiveButton;
        [SerializeField] private GameObject cauldronStandart;
        [SerializeField] private GameObject cauldronPremium;
        [SerializeField] private SkinSO skinPremium;

        private Image image;

        [Inject] private GameDataHandler gameData;
        [Inject] private Cauldron cauldron;
        [Inject] private WitchSkinChanger witch;
        [Inject] private CatAnimations cat;
        
        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void Start()
        {
            StartCoroutine(UpdateSkins());
        }
        
        private IEnumerator UpdateSkins()
        {
            UpdateButton();
            UpdateWitch();
            UpdateCat();
            yield return new WaitForSeconds(animationDelay);
            UpdateCauldron();
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
            witch.ChangeSkin(gameData.premiumSkin ? "Premium" : gameData.currentSkin.SpineName);
        }

        private void UpdateCat()
        {            
            cat.SetSkin(gameData.premiumSkin ? "ArchCat" : "Cat_default");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            gameData.premiumSkin = !gameData.premiumSkin;
            StopAllCoroutines();
            StartCoroutine(UpdateSkins());
        }
    }
}