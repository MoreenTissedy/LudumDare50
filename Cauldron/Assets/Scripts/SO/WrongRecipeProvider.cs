using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "WrongRecipeProvider", menuName = "WrongRecipeProvider")]
    public class WrongRecipeProvider : ScriptableObject
    {
        private const string WrongRecipeKey = "WrongRecipe";
        private FileDataHandler<WrongRecipeProvider> fileDataHandler;

        public List<WrongPotion> wrongPotions = new List<WrongPotion>();

        private void TryInitDataHandler()
        {
            if (fileDataHandler != null) return;

            fileDataHandler  = new FileDataHandler<WrongRecipeProvider>(WrongRecipeKey);
        }

        public void ResetWrongRecipe()
        {
            wrongPotions.Clear();
            SaveWrongRecipes();
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
                fileDataHandler.LoadWithOverwrite(this);
            }
            return wrongPotions;
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
                
                list = wrongPotions;
                return true;
            }

            list = null;
            return false;
        }
    }
}