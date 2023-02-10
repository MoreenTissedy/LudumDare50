using System;
using UnityEngine;

namespace EasyLoc
{
    public abstract class LocalizableSO : ScriptableObject, ILocalize
    {
        public TextAsset localizationCSV;
        
        public abstract bool Localize(Language language);
    }
}