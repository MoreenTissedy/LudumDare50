using System;
using UnityEngine;
using UnityEngine.UI;
using EasyLoc;
using DG.Tweening;
using Zenject;
using TMPro;
using Universal;

namespace CauldronCodebase
{
    public class PotionPopup : MonoBehaviour
    {
        private const float EFFECT_DURATION = 1.5f;
        [Localize]
        public string youBrewed = "Вы сварили: ";
        [Localize]
        public string noRecipeForThis = "Вы сварили что-то не то";
        public TMP_Text wording;
        public GameObject pictureContainer;
        public Image picture;
        public Sprite defaultPicture;
        public GameObject newPotionEffect;
        public float tweenDuration = 0.3f;
        public float startTweenSize = 0f;
        public FlexibleButton accept;
        public FlexibleButton decline;
        public OverlayLayer uiLayer;
        
        [Inject] private SoundManager soundManager;
        [Inject] private InputManager inputManager;
        [Inject] private OverlayManager overlayManager;

        public bool IsEnable { get; private set; }
        
        public event Action OnAccept;
        public event Action OnDecline;

        private void Start()
        {
            gameObject.SetActive(false);
            accept.OnClick += Accept;
            decline.OnClick += Decline;
        }

        public void Show(Recipe recipe, bool newPotion = false)
        {
            IsEnable = true;
            gameObject.SetActive(true);
            overlayManager.AddLayer(uiLayer);
            newPotionEffect.SetActive(false);
            inputManager.SetCursor(false);
            transform.DOScale(1, tweenDuration).From(startTweenSize).
                OnComplete(() =>
                {
                    newPotionEffect.SetActive(newPotion);
                    if (newPotion)
                    {
                        soundManager.Play(Sounds.PotionUnlock);
                    }
                }).
                SetDelay(EFFECT_DURATION);
            
            if (recipe is null)
            {
                wording.text = noRecipeForThis;
                pictureContainer.SetActive(false);
            }
            else
            {
                pictureContainer.SetActive(true);
                wording.text = youBrewed + recipe.potionName;
                picture.sprite = recipe.image;
            }
            //TODO remove accept button if no visitor
        }

        private void Accept()
        {
            Hide();
            OnAccept?.Invoke();
        }

        private void Decline()
        {
            Hide();
            OnDecline?.Invoke();
        }

        public void ClearAcceptSubscriptions()
        {
            OnAccept = null;
        }

        void Hide()
        {
            inputManager.SetCursor(true);
            soundManager.Play(Sounds.PotionPopupClick);
            newPotionEffect.SetActive(false);
            transform.DOScale(startTweenSize, tweenDuration)
                .OnComplete(() => gameObject.SetActive(false));

            IsEnable = false;
            overlayManager.RemoveLayer(uiLayer);
        }

        private void OnDestroy()
        {
            accept.OnClick -= Accept;
            decline.OnClick -= Decline;
        }
    }
}