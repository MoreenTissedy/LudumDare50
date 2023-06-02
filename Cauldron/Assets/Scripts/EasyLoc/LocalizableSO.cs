using System;
using CauldronCodebase;
using UnityEngine;

namespace EasyLoc
{
    public abstract class LocalizableSO : ScriptableObjectInDictionary, ILocalize
    {
        public TextAsset localizationCSV;
        
        public abstract bool Localize(Language language);
    }
}