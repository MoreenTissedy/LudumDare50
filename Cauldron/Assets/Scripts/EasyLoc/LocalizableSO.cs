using CauldronCodebase;
using UnityEngine;

namespace EasyLoc
{
    public abstract class LocalizableSO : ScriptableObjectWithId, ILocalize
    {
        public TextAsset localizationCSV;
        
        public abstract bool Localize(Language language);
    }
}