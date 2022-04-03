using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class PotionPopup : MonoBehaviour
    {
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
                wording.text = "Вы сварили: " + recipe.potionName;
                //picture.sprite = recipe.image;
            }
            else
            {
                wording.text = "Зелье не получилось";
                picture.sprite = defaultPicture;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}