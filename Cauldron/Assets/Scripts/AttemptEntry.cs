using System.Collections.Generic;
using EasyLoc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class AttemptEntry : MonoBehaviour
    {
        [Inject]
        private IngredientsData data;
        
        [SerializeField] private Image ingredient1Image, ingredient2Image, ingredient3Image;
        [SerializeField] private Image negativeResultImage;
        [SerializeField] private Image unknownResultImage;
        [SerializeField] private Image potionResultImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [Localize] public string failure;
        [Localize] public string notTried;

        private void Awake()
        {
            Clear();
        }

        public void DisplayFailure(Ingredients[] attempt)
        {
            if (attempt.Length != 3)
            {
                return;
            }
            
            ingredient1Image.sprite = data.Get(attempt[0]).image;
            ingredient2Image.sprite = data.Get(attempt[1]).image;
            ingredient3Image.sprite = data.Get(attempt[2]).image;
            titleText.text = failure;
            negativeResultImage.gameObject.SetActive(true);
            potionResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
       
        public void DisplayPotion(Ingredients[] attempt, Recipe recipe)
        {
            if (attempt.Length != 3)
            {
                return;
            }
            
            ingredient1Image.sprite = data.Get(attempt[0]).image;
            ingredient2Image.sprite = data.Get(attempt[1]).image;
            ingredient3Image.sprite = data.Get(attempt[2]).image;
            titleText.text = recipe.potionName;
            potionResultImage.sprite = recipe.image;
            potionResultImage.gameObject.SetActive(true);
            negativeResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public void DisplayNotTried(Ingredients[] attempt)
        {
            if (attempt.Length != 3)
            {
                return;
            }
            
            ingredient1Image.sprite = data.Get(attempt[0]).image;
            ingredient2Image.sprite = data.Get(attempt[1]).image;
            ingredient3Image.sprite = data.Get(attempt[2]).image;
            titleText.text = notTried;
            negativeResultImage.gameObject.SetActive(false);
            potionResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            negativeResultImage.gameObject.SetActive(false);
            unknownResultImage.gameObject.SetActive(false);
        }
    }
}