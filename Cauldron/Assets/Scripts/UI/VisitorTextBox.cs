using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class VisitorTextBox : MonoBehaviour
    {
        const string DEVIL = "devil";
        
        public float offScreen = -1000;
        public float animTime = 0.5f;
        public TMP_Text text;
        public VisitorTextIcon[] iconObjects = new VisitorTextIcon[3];
        public Image timer;
        public float timerChangeSpeed = 1f;
        [Inject] private RecipeBook recipeBook;
        [Inject] private RecipeProvider recipeProvider;
        
        [ContextMenu("Find Icon Objects")]
        private void FindIconObjects()
        {
            iconObjects = GetComponentsInChildren<VisitorTextIcon>();
        }

        public void ReduceTimer(float value)
        {
            var newFillAmount = Mathf.Clamp(timer.fillAmount - value, 0, 1);
            timer.DOFillAmount(newFillAmount, timerChangeSpeed);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        } 
        
        public void Display(Encounter card)
        {
            gameObject.SetActive(true);
            timer.fillAmount = 1f;
            gameObject.transform.DOLocalMoveX(gameObject.transform.localPosition.x, animTime)
                .From(offScreen);
            
            if (card.name.Contains(DEVIL))
            {
                //what if everything is unlocked?
                Recipe unlockRecipe = GetRecipeToUnlock();
                text.text = String.Format(card.text, unlockRecipe.ingredient1, unlockRecipe.ingredient2, unlockRecipe.ingredient3);
            }
            else
            {
                text.text = card.text;
            }
            
            iconObjects[0]?.Display(card.primaryInfluence, card.hidden);
            iconObjects[1]?.Display(card.secondaryInfluence, card.hidden);
            if (card.quest)
            {
                iconObjects[2]?.DisplayItem();
                text.fontStyle = FontStyles.Italic;
            }
            else
            {
                iconObjects[2]?.Hide();
                text.fontStyle = FontStyles.Normal;
            }
        }
        
        //move?
        private Recipe GetRecipeToUnlock()
        {
            return recipeProvider.allRecipes.
                Where(x => x.magical).
                FirstOrDefault(x => !recipeBook.IsRecipeInBook(x));
        }
    }
}