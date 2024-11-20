using UnityEngine;
using EasyLoc;

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
        public SkinSO unlocksSkin;
        public override bool Localize(Language language)
        {
            if (localizationCSV == null)
                return false;
            //cache??
            string[] lines = localizationCSV.text.Split('\n');
            int requiredColumn = -1;
            string[] headers = lines[0].Split(';');
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Contains("_"+language.ToString()))
                {
                    requiredColumn = i;
                    break;
                }
            }
            if (requiredColumn < 1)
            {
                return false;
            }

            bool found = false;
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(';');
                if (data[0] == $"{name}.title")
                {
                    title = data[requiredColumn];
                    found = true;
                }
                else if (data[0] == $"{name}.description")
                {
                    text = data[requiredColumn];
                    found = true;
                }
                else if (data[0] == $"{name}.short")
                {
                    shortTextForEndingAnimation = data[requiredColumn];
                }
            }
            return found;
        }
    }
}