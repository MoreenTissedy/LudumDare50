using UnityEngine;
using EasyLoc;
using System.Collections.Generic;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New_ending", menuName = "Ending", order = 9)]
    public class Ending : LocalizableSO
    {
        public string tag;
        public string title;
        [TextArea(3, 10)]
        public string text;
        public string shortTextForEndingAnimation;
        public Sprite image;
        public Sprite endIconImage;
        public override bool Localize(Language language)
        {
            if (localizationCSV == null)
                return false;
            //cache??
            string[] lines = localizationCSV.text.Split('\n');
            List<int> requiredColumns = new List<int>();
            string[] headers = lines[0].Split(';');
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+language.ToString()))
                {
                    requiredColumns.Add(i);
                }
            }

            int line = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (data[0].StartsWith(name))
                {
                    string lineText = data[requiredColumns[line]];
                    if (line == 0)
                    {
                        title = lineText;
                    }
                    else if (line == 1)
                    {
                        text = lineText;
                    }
                    else
                    {
                        shortTextForEndingAnimation = lineText;
                    }
                    line++;
                }
            }
            return line>0;
        }
    }
}