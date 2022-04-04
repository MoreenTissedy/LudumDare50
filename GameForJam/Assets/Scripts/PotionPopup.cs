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
                picture.sprite = recipe.image;
            //    picture.color = recipe.color;
            }
            else
            {
                wording.text = "Вы сварили что-то не то";
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