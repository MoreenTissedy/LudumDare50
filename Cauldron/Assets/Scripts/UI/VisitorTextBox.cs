using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EasyLoc;
using TMPro;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class VisitorTextBox : MonoBehaviour
    {
        [Localize]
        public string devilDefault = "Привет, милая. Как делишки? Покажи-ка, что ты умеешь.";
        public float offScreen = -1000;
        public float animTime = 0.5f;
        public TMP_Text text;
        public VisitorTextIcon[] iconObjects = new VisitorTextIcon[3];
        
        [Inject] private RecipeBook recipeBook;
        [Inject] private RecipeProvider recipeProvider;
        [Inject] private IngredientsData ingredients;
        [Inject] private LocalizationTool locTool;
        [Inject] private GameDataHandler gameDataHandler;

        private Encounter currentEncounter;

        
        [ContextMenu("Find Icon Objects")]
        private void FindIconObjects()
        {
            iconObjects = GetComponentsInChildren<VisitorTextIcon>();
        }

        private void Start()
        {
            locTool.OnLanguageChanged += ReloadVisitorText;
        }

        private void OnDestroy()
        {
            locTool.OnLanguageChanged -= ReloadVisitorText;
        }

        private void ReloadVisitorText(Language lang)
        {
            if (currentEncounter != null)
            {
                Display(currentEncounter);
                //fix devil?
            }
        }

        public void Hide()
        {
            currentEncounter = null;
            gameObject.SetActive(false);
        } 
        
        public void Display(Encounter card)
        {
            gameObject.transform.DOLocalMoveX(gameObject.transform.localPosition.x, animTime)
                .From(offScreen);

            currentEncounter = card;
            if (card.villager.name.Contains(EncounterIdents.DARK_STRANGER))
            {
                Recipe unlockRecipe = recipeProvider.GetRecipeToUnlock(recipeBook, gameDataHandler);
                if (unlockRecipe == null)
                {
                    text.text = devilDefault;
                }
                else
                {
                    text.text = Format(card, unlockRecipe);
                }
            }
            else if (card.name.Contains(EncounterIdents.CAT_UNLOCK))
            {
                Recipe unlockRecipe = recipeProvider.GetRecipeToUnlock(recipeBook, gameDataHandler);
                text.text = Format(card, unlockRecipe);
            }
            else
            {
                text.text = card.text;
            }

            iconObjects[0]?.Display(card.primaryInfluence, card.hidden);
            iconObjects[1]?.Display(card.secondaryInfluence, card.hidden);
            if (card.fraction != Fractions.None)
            {
                iconObjects[2]?.DisplayFraction(card.fraction);
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
    }
}