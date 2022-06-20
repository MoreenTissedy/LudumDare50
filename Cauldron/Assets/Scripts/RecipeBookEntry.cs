using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class RecipeBookEntry : MonoBehaviour
    {
        public Text fullName;
        public Text description;
        public Image image;
        public Image ingredient1, ingredient2, ingredient3;
        private Recipe currentRecipe;
        public Recipe CurrentRecipe => currentRecipe;

        private IngredientsData ingredientsData;
           
        [Inject]
        public void Construct(IngredientsData data)
        {
            this.ingredientsData = data;
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
    //        image.color = recipe.color;
            ingredient1.sprite = ingredientsData.Get(recipe.ingredient1).image;
            ingredient2.sprite = ingredientsData.Get(recipe.ingredient2).image;
            ingredient3.sprite = ingredientsData.Get(recipe.ingredient3).image;
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