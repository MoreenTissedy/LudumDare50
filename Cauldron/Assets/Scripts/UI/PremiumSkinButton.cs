using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class PremiumSkinButton : AnimatedButton
    {
        [SerializeField] private float animationDelay = 0.15f;
        [SerializeField] private Sprite activeButton;
        [SerializeField] private Sprite notActiveButton;
        [SerializeField] private GameObject cauldronStandart;
        [SerializeField] private GameObject cauldronPremium;
        [SerializeField] private Image image;

        [Inject] private GameDataHandler gameData;
        [Inject] private Cauldron cauldron;
        [Inject] private WitchSkinChanger witch;
        [Inject] private CatAnimations cat;

        public void Start()
        {
            gameObject.SetActive(SteamConnector.HasPremium);
            if (gameData.premiumSkin)
            {
                StartCoroutine(UpdateSkins());
            }
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

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            gameData.premiumSkin = !gameData.premiumSkin;
            StopAllCoroutines();
            StartCoroutine(UpdateSkins());
        }
    }
}