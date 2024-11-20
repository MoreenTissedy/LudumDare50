using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CauldronCodebase
{
    public class WrongRecipeProvider
    {
        private const string WrongRecipeKey = "WrongRecipe";
        private FileDataHandler<WrongRecipeProvider> fileDataHandler;

        public List<WrongPotion> WrongPotions = new List<WrongPotion>();

        private void TryInitDataHandler()
        {
            if (fileDataHandler != null) return;

            fileDataHandler  = new FileDataHandler<WrongRecipeProvider>(WrongRecipeKey);
        }

        public void SaveWrongRecipes()
        {
            TryInitDataHandler();
            fileDataHandler.Save(this);
        }

        public List<WrongPotion> LoadWrongRecipe()
        {
            if (TryLoadLegacy(out var list)) return list;

            TryInitDataHandler();
            if (fileDataHandler.IsFileValid())
            {
                return fileDataHandler.Load().WrongPotions;
            }
            return new List<WrongPotion>();
        }

        private bool TryLoadLegacy(out List<WrongPotion> list)
        {
            if (File.Exists(Application.persistentDataPath + WrongRecipeKey))
            {
                string saveData = File.ReadAllText(Application.persistentDataPath + WrongRecipeKey);
                JsonUtility.FromJsonOverwrite(saveData, this);
                
                TryInitDataHandler();
                fileDataHandler.Save(this);
                File.Delete(Application.persistentDataPath + WrongRecipeKey);
                
                list = WrongPotions;
                return true;
            }

            list = null;
            return false;
        }
    }
}