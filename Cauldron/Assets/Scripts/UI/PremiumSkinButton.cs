using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class PremiumSkinButton : MonoBehaviour
    {
        [SerializeField] private float animationDelay = 0.15f;
        [SerializeField] private Sprite activeButton;
        [SerializeField] private Sprite notActiveButton;
        [SerializeField] private GameObject cauldronStandart;
        [SerializeField] private GameObject cauldronPremium;
        [SerializeField] private Image image;
        [SerializeField] private FlexibleButton button;

        [Inject] private GameDataHandler gameData;
        [Inject] private Cauldron cauldron;
        [Inject] private WitchSkinChanger witch;
        [Inject] private CatAnimations cat;

        public void Start()
        {
            button.OnClick += ChangeSkins;
            
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

        public void ChangeSkins()
        {
            gameData.premiumSkin = !gameData.premiumSkin;
            StopAllCoroutines();
            StartCoroutine(UpdateSkins());
        }

        private void OnDestroy()
        {
            button.OnClick -= ChangeSkins;
        }
    }
}