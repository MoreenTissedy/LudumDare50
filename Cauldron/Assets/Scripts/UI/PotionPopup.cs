using System;
using UnityEngine;
using UnityEngine.UI;
using EasyLoc;
using DG.Tweening;
using Save;
using Zenject;
using TMPro;
using Zenject;

namespace CauldronCodebase
{
    public class PotionPopup : MonoBehaviour
    {
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


        [Inject] private DataPersistenceManager dataPersistenceManager;

        [Inject] private SoundManager soundManager;

        
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
            gameObject.SetActive(true);
            newPotionEffect.SetActive(false);
            transform.DOScale(1, tweenDuration).From(startTweenSize).
                OnComplete(() => newPotionEffect.SetActive(newPotion)).
                SetDelay(PotionExplosion.EFFECT_DURATION);
            
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
            
            dataPersistenceManager.SaveGame();
        }
    }
}