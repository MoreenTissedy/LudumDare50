using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookEntry : MonoBehaviour
    {
        private HashSet<Ingredients> lockedIngredients = new HashSet<Ingredients>() {Ingredients.Agaricus, Ingredients.Root1};
        
        public Text fullName;
        public Text description;
        public Image image;
        public Image ingredient1, ingredient2, ingredient3;
        public PotionSelect potionButton;
        private Recipe currentRecipe;
        public Recipe CurrentRecipe => currentRecipe;

        private IngredientsData ingredientsData;

        [SerializeField]
        private Material lockedMaterial;
           
        [Inject]
        public void Construct(IngredientsData data)
        {
            ingredientsData = data;
            Clear();
        }
        
        public void Display(Recipe recipe)
        {
            currentRecipe = recipe;
            image.enabled = true;
            ingredient1.enabled = true;
            ingredient2.enabled = true;
            ingredient3.enabled = true;
            fullName.text = recipe.potionName;
            description.text = recipe.description;
            image.sprite = recipe.image;
            ingredient1.sprite = ingredientsData.Get(recipe.RecipeIngredients[0]).image;
            ingredient2.sprite = ingredientsData.Get(recipe.RecipeIngredients[1]).image;
            ingredient3.sprite = ingredientsData.Get(recipe.RecipeIngredients[2]).image;
            image.material = null;
            potionButton.clickable = true;
        }

        public void DisplayLocked(Recipe recipe)
        {
            currentRecipe = null;
            image.enabled = true;
            ingredient1.enabled = false;
            ingredient2.enabled = false;
            ingredient3.enabled = false;
            fullName.text = "???";
            description.text = IsRecipeUnavailable(recipe) ? "Unavailable in demo" : "";
            image.sprite = recipe.image;
            image.material = lockedMaterial;
            potionButton.clickable = false;
        }

        private bool IsRecipeUnavailable(Recipe recipe)
        {
            foreach (var ingr in recipe.RecipeIngredients)
            {
                if (lockedIngredients.Contains(ingr))
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            currentRecipe = null;
            fullName.text = "";
            description.text = "";
            image.enabled = false;
            ingredient1.enabled = false;
            ingredient2.enabled = false;
            ingredient3.enabled = false;
        }

    }
}