using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class PotionPopup : MonoBehaviour
    {
        public string youBrewed = "Вы сварили: ";
        public string noRecipeForThis = "Вы сварили что-то не то";
        public Text wording;
        public Image picture;
        public Sprite defaultPicture;

        private void Start()
        {
            Hide();
        }

        public void Show(Recipe recipe)
        {
            gameObject.SetActive(true);
            if (recipe != null)
            {
                wording.text = youBrewed + recipe.potionName;
                picture.sprite = recipe.image;
            //    picture.color = recipe.color;
            }
            else
            {
                wording.text = noRecipeForThis;
                picture.sprite = defaultPicture;
           //     picture.color = new Color32(255,255,255,100);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}