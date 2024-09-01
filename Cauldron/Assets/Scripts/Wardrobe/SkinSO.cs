using EasyLoc;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "SkinSo")]
    public class SkinSO : LocalizableSO
    {
        public string SkinName;
        
        public Sprite PreviewIcon;
        public Sprite FullPreview;
        
        public string FlavorText;
        public string DescriptionText;

        public int Price;
        
        public string[] LastUnlockedEnding;
        
        public override bool Localize(Language language)
        {
            return true;
        }
    }
}
