using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class IngredientView : MonoBehaviour
{
    [SerializeField] private Image ingredientImage;
    [SerializeField] private float fadeInTime, fadeOutTime, scaleTime;

    public bool IngredientShown { get; private set; }

    private Sequence hideSequence;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        ingredientImage.sprite = null;
        ingredientImage.color = new Color(1, 1, 1, 0);
        IngredientShown = false;

        hideSequence = DOTween.Sequence().Pause().SetAutoKill(false);
        hideSequence.Append(rectTransform.DOScale(1.2f, scaleTime/2).SetEase(Ease.OutSine).SetSpeedBased())
                    .Append(rectTransform.DOScale(1f, scaleTime).SetEase(Ease.OutSine).SetSpeedBased())
                    .Append(ingredientImage.DOFade(0, fadeOutTime).SetEase(Ease.OutQuint))
                    .OnComplete(() =>
                    {
                        ingredientImage.sprite = null;
                        IngredientShown = false;
                    });
    }

    public void SetImage(Sprite sprite)
    {
        
        IngredientShown = true;
        ingredientImage.sprite = sprite;
        if (ingredientImage.color.a == 0)
        {
            ingredientImage.DOFade(1, fadeInTime).SetEase(Ease.OutQuint);
        }
    }

    public void HideImage(bool hideWithoutAnimation = false)
    {
        if (hideWithoutAnimation)
        {
            ingredientImage.color = new Color(1, 1, 1, 0);
            ingredientImage.sprite = null;
            IngredientShown = false;
        }
        else
        {
            hideSequence.Restart();
        }
    }
}
