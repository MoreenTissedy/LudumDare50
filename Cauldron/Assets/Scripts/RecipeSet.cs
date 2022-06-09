using UnityEditor;
using UnityEngine;

namespace CauldronCodebase
{
    public class RecipeSet : MonoBehaviour
    {

        public static RecipeSet instance;
        public Recipe[] allRecipes;

        void Awake()
        {
            if (instance is null)
                instance = this;
        }
    }
}