using System;
using System.IO;
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

            [Header("For ingredients book")] 
            public Sprite TopImage;
            public Sprite BottomImage;
            public string TextInABook;
        }
        
        public Ingredient[] book;

        public Ingredient Get(Ingredients type)
        {
            return (book.Where(x => x.type == type).ToArray()[0]);
        }
        
        [ContextMenu("Export ingredients data to csv")]
        public void ExportAllNightEvents()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Ingredients.csv");
            file.WriteLine("type;title_RU;title_EN;description_RU;description_EN");
            foreach (var element in book)
            {
                file.WriteLine($"{element.type};{element.friendlyName};;{element.TextInABook}");
            }
            file.Close();
        }
    }
}