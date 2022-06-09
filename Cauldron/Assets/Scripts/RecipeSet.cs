using UnityEditor;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "All Recipe Set", menuName = "Recipe Deck", order = 1)]
    public class RecipeSet : ScriptableObject
    {
        public Recipe[] allRecipes;
    }
}