using EasyLoc;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "SkinSo")]
    public class SkinSO : LocalizableSO
    {
        public string SkinName;
        public Sprite Preview;
        public string Text;
        public bool IsAvailable;
        public string[] LastUnlockedEnding;
        public override bool Localize(Language language)
        {
            return true;
        }
    }
}
