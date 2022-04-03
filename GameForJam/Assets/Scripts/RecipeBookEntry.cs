using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class RecipeBookEntry : MonoBehaviour
    {
        [FormerlySerializedAs("name")] public Text fullName;
        public Text description;
        public Image image;
        public Image ingredient1, ingredient2, ingredient3;

        public void Display(Recipe recipe)
        {
            fullName.text = recipe.potionName;
            description.text = recipe.description;
            image.sprite = recipe.image;
            ingredient1.sprite = GameManager.instance.ingredientsBook.Get(recipe.ingredient1).image;
            ingredient2.sprite = GameManager.instance.ingredientsBook.Get(recipe.ingredient2).image;
            ingredient3.sprite = GameManager.instance.ingredientsBook.Get(recipe.ingredient3).image;
        }

        public void Clear()
        {
            fullName.text = "";
            description.text = "";
            image.sprite = null;
            ingredient1.sprite = null;
            ingredient2.sprite = null;
            ingredient3.sprite = null;
        }
    }
}