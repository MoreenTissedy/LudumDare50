using System;
using UnityEngine;
using UnityEngine.UI;
using EasyLoc;
using DG.Tweening;
using Save;
using Zenject;
using TMPro;

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
        public Button accept;
        public Button decline;
        
        [Inject] private SoundManager soundManager;

        public bool IsEnable { get; private set; }
        
        public event Action OnAccept;
        public event Action OnDecline;

        private void Start()
        {
            gameObject.SetActive(false);
            accept.onClick.AddListener(Accept);
            decline.onClick.AddListener(Decline);
        }

        public void Show(Recipe recipe, bool newPotion = false)
        {
            IsEnable = true;
            gameObject.SetActive(true);
            newPotionEffect.SetActive(false);
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
            OnAccept?.Invoke();
            Hide();
        }

        private void Decline()
        {
            OnDecline?.Invoke();
            Hide();
        }

        public void ClearAcceptSubscriptions()
        {
            OnAccept = null;
        }

        void Hide()
        {
            soundManager.Play(Sounds.PotionPopupClick);
            newPotionEffect.SetActive(false);
            transform.DOScale(startTweenSize, tweenDuration)
                .OnComplete(() => gameObject.SetActive(false));

            IsEnable = false;
        }
    }
}