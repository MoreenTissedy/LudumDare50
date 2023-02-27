using System;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
        
        [Inject] private RecipeBook recipeBook;
        [Inject] private RecipeProvider recipeProvider;
        [Inject] private IngredientsData ingredients;

        
        [ContextMenu("Find Icon Objects")]
        private void FindIconObjects()
        {
            iconObjects = GetComponentsInChildren<VisitorTextIcon>();
        }

        

        public void Hide()
        {
            gameObject.SetActive(false);
        } 
        
        public void Display(Encounter card)
        {
            gameObject.transform.DOLocalMoveX(gameObject.transform.localPosition.x, animTime)
                .From(offScreen);

            if (card.name.Contains(DEVIL))
            {
                //what if everything is unlocked?
                Recipe unlockRecipe = GetRecipeToUnlock();
                if (unlockRecipe == null)
                {
                    text.text = "Свари мне что нибудь";
                }
                text.text = Format(card, unlockRecipe);
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

            string Format(Encounter encounter, Recipe unlockRecipe)
            {
                string name1 = ingredients.Get(unlockRecipe.RecipeIngredients[0]).friendlyName.ToLower();
                string name2 = ingredients.Get(unlockRecipe.RecipeIngredients[1]).friendlyName.ToLower();
                string name3 = ingredients.Get(unlockRecipe.RecipeIngredients[2]).friendlyName.ToLower();
                return String.Format(encounter.text, name1, name2, name3);
            }

            gameObject.SetActive(true);
        }
        
        //move?
        private Recipe GetRecipeToUnlock()
        {
            Recipe recipe = recipeProvider.allRecipes.
                Where(x => x.magical).
                FirstOrDefault(x => !recipeBook.IsRecipeInBook(x));
            
            Debug.Log($"Get recipe to unlock {recipe}");
            return recipe;
        }
    }
}