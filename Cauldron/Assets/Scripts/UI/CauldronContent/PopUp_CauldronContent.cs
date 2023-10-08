using System;
using CauldronCodebase;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class PopUp_CauldronContent : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private float hiddenPosition;
    [SerializeField] private float shownPosition;
    
    [Header("Time")]
    [SerializeField] private float showSpeed;
    [SerializeField] private float showTime;
    [SerializeField] private float hideSpeed;
    
    [SerializeField] private IngredientView[] ingredients;

    private RectTransform rectTransform;
    
    private Cauldron cauldron;
    private IngredientsData ingredientsData;
    private RecipeBook recipeBook;

    [Inject]
    private void Construct(Cauldron cauldron, IngredientsData ingredientsData, RecipeBook recipeBook)
    {
        this.cauldron = cauldron;
        this.ingredientsData = ingredientsData;
        this.recipeBook = recipeBook;
    }

    private void Start()
    {
        recipeBook.OnSelectIncorrectRecipe += TriggerPopUp;
        
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, hiddenPosition);
    }

    private void OnDestroy()
    {
        recipeBook.OnSelectIncorrectRecipe -= TriggerPopUp;
    }

    private void TriggerPopUp()
    {
        if(Math.Abs(rectTransform.anchoredPosition.y - hiddenPosition) > 1) return;

        UpdateIngredientSprites();

        Sequence popupSequence = DOTween.Sequence();
        popupSequence.Append(rectTransform.DOAnchorPosY(shownPosition, showSpeed).SetEase(Ease.OutQuint))
                     .AppendInterval(showTime)
                     .Append(rectTransform.DOAnchorPosY(hiddenPosition, hideSpeed).SetEase(Ease.OutQuad));

    }

    private void UpdateIngredientSprites()
    {
        for (int i = 0; i < ingredients.Length; i++)
        {
            if (i < cauldron.Mix.Count)
            {
                ingredients[i].SetImage(ingredientsData.Get(cauldron.Mix[i]).image);
            }
            else
            {
                ingredients[i].HideImage(true);
            }
        }
    }
}
