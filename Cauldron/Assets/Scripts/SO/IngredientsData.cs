using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyLoc;
using UnityEditor;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "Ingredients Book", menuName = "Ingredients", order = 0)]
    public class IngredientsData : LocalizableSO
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
            [TextArea(7, 30)]
            public string TextInABook;
        }
        
        public Ingredient[] book;

        public Ingredient Get(Ingredients type)
        {
            return (book.Where(x => x.type == type).ToArray()[0]);
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Export ingredients data to csv")]
        public void ExportIngredientsData()
        {
            var path = "/Localize/Ingredients.csv";
            var file = File.CreateText(Application.dataPath+path);
            file.WriteLine("type;title_RU;title_EN;description_RU;description_EN");
            foreach (var element in book)
            {
                file.WriteLine($"{element.type};{element.friendlyName};;{element.TextInABook}");
            }
            file.Close();
            localizationCSV = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets"+path);
        }

        [ContextMenu("Localize_EN")]
        public void LocEn()
        {
            Localize(Language.EN);
        }
        [ContextMenu("Localize_RU")]
        public void LocRu()
        {
            Localize(Language.RU);
        }
        #endif
        
        public override bool Localize(Language language)
        {
            if (localizationCSV == null)
                return false;
            string[] lines = localizationCSV.text.Split('\n');
            List<int> requiredColumns = new List<int>();
            string[] headers = lines[0].Split(';');
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+language))
                {
                    requiredColumns.Add(i);
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (book[i - 1].type.ToString() != data[0])
                {
                    Debug.LogError($"Mismatched ingredient indices at line {i}");
                    continue;
                }
                book[i - 1].friendlyName = data[requiredColumns[0]];
                book[i - 1].TextInABook = data[requiredColumns[1]];
            }

            return true;
        }
    }
}