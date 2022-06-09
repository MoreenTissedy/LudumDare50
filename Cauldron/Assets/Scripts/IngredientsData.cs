using System;
using System.Linq;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Ingredients Book", menuName = "Ingredients", order = 0)]
    public class IngredientsData : ScriptableObject
    {
        [Serializable]
        public class Ingredient
        {
            public Ingredients type;
            public string friendlyName;
            public Sprite image;
        }
        
        public Ingredient[] book;

        public Ingredient Get(Ingredients type)
        {
            return (book.Where(x => x.type == type).ToArray()[0]);
        }
    }
}