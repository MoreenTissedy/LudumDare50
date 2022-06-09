using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using EasyLoc;

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
        public float popupDuration = 2f;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Show(Recipe recipe)
        {
            if (recipe is null)
            {
                ShowFailure();
                return;
            }

            gameObject.SetActive(true);
            wording.text = youBrewed + recipe.potionName;
            picture.sprite = recipe.image;
            StartCoroutine(Hide());
        }

        public void ShowFailure()
        {
            gameObject.SetActive(true);
            wording.text = noRecipeForThis;
            picture.sprite = defaultPicture;
            StartCoroutine(Hide());
        }

        IEnumerator Hide()
        {
            yield return new WaitForSeconds(popupDuration);
            gameObject.SetActive(false);
        }
    }
}