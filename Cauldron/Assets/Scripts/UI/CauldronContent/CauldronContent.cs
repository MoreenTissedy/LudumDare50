using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class CauldronContent : MonoBehaviour
    {
        [SerializeField] private float shownPosition, hiddenPosition;
        [SerializeField] private float panelShowSpeed, panelHideSpeed, panelHideDelay;
        [SerializeField] private IngredientView[] ingredientViewObjects;
        [SerializeField] private RectTransform contentTransform;

        [Inject] private Cauldron cauldron;
        [Inject] private IngredientsData ingredientsData;

        private void Start()
        {
            contentTransform.anchoredPosition = new Vector2(0, hiddenPosition);
            
            cauldron.IngredientAdded += ShowContent;
            cauldron.PotionBrewed += HideContent;
        }

        private void OnDestroy()
        {
            cauldron.IngredientAdded -= ShowContent;
            cauldron.PotionBrewed -= HideContent;
        }

        private void ShowContent(Ingredients ingredient)
        {
            if (Math.Abs(contentTransform.anchoredPosition.y - shownPosition) > 0.1)
            {
                contentTransform.DOAnchorPosY(shownPosition, panelShowSpeed);
            }

            for (int i = 0; i < cauldron.Mix.Count; i++)
            {
                if(ingredientViewObjects[i].IngredientShown) continue;
                
                ingredientViewObjects[i].SetImage(ingredientsData.Get(cauldron.Mix[i]).image);
            }
        }

        private void HideContent(Potions potions)
        {
            foreach (var obj in ingredientViewObjects)
            {
                obj.HideImage();
            }

            contentTransform.DOAnchorPosY(hiddenPosition, panelHideSpeed)
                .SetDelay(panelHideDelay).SetEase(Ease.OutExpo);

        }
    }
}
