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
        
        [Header("Legacy compatibility")]
        public string[] LastUnlockedEnding;

        public override bool Localize(Language language)
        {
            return true;
        }
    }
}
