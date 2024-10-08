using System.IO;
using EasyLoc;
using NaughtyAttributes;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "SkinSo")]
    public class SkinSO : LocalizableSO
    {
        public string SpineName;
        public string FriendlyName;
        
        [ShowAssetPreview()]
        public Sprite PreviewIcon;
        
        [TextArea(10, 20)]
        public string FlavorText;
        [TextArea(5, 20)]
        public string DescriptionText;

        public int Price;

        public GameModeBase GameMode;

        public bool NeedsApprove;

        [ShowIf("NeedsApprove")]
        [TextArea(5, 20)]
        public string ApproveMessage;
        
        [Header("Legacy compatibility")]
        public string[] LastUnlockedEnding;

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
                    FriendlyName = data[requiredColumn];
                    found = true;
                }
                else if (data[0] == $"{name}.description")
                {
                    FlavorText = data[requiredColumn];
                    found = true;
                }
                else if (data[0] == $"{name}.short")
                {
                    DescriptionText = data[requiredColumn];
                }
            }
            return found;
        }
    }
}
