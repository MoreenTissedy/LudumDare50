using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EasyLoc;
using DG.Tweening;

namespace CauldronCodebase
{
    public class PotionPopup : MonoBehaviour
    {
        [Localize]
        public string youBrewed = "Вы сварили: ";
        [Localize]
        public string noRecipeForThis = "Вы сварили что-то не то";
        public Text wording;
        public Image picture;
        public Sprite defaultPicture;
        public GameObject newPotionEffect;
        public float popupDuration = 2f;
        public float tweenDuration = 0.3f;
        public float startTweenSize = 0.3f;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show(Recipe recipe, bool newPotion = false)
        {
            gameObject.SetActive(true);
            transform.DOScale(1, tweenDuration).From(startTweenSize);
            newPotionEffect.SetActive(newPotion);
            
            if (recipe is null)
            {
                wording.text = noRecipeForThis;
                picture.sprite = defaultPicture;
            }
            else
            {
                wording.text = youBrewed + recipe.potionName;
                picture.sprite = recipe.image;
            }
            StartCoroutine(Hide());
        }


        IEnumerator Hide()
        {
            yield return new WaitForSeconds(popupDuration);
            transform.DOScale(startTweenSize, tweenDuration)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}