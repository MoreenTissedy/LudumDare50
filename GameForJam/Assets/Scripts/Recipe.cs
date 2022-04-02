using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New recipe", menuName = "Recipe", order = 0)]
    public class Recipe : ScriptableObject
    {
        public Potions potion;
        public Ingredients ingredient1, ingredient2, ingredient3;
        public string potionName;
        public string description;
        public Sprite image;
    }
}