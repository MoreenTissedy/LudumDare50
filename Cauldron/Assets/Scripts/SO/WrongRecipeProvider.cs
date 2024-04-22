using System.Collections.Generic;
using System.IO;
using Save;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "WrongRecipeProvider", menuName = "WrongRecipeProvider")]
    public class WrongRecipeProvider : ScriptableObject
    {
        private const string WrongRecipeKey = "WrongRecipe";

        private readonly FileDataHandler<WrongRecipeProvider> fileDataHandler =
            new FileDataHandler<WrongRecipeProvider>("WrongRecipe");

        public List<WrongPotion> wrongPotions = new List<WrongPotion>();

        public void ResetWrongRecipe()
        {
            wrongPotions.Clear();
            SaveWrongRecipes();
        }

        public void SaveWrongRecipes()
        {
            fileDataHandler.Save(this);
        }

        public List<WrongPotion> LoadWrongRecipe()
        {
            if (TryLoadLegacy(out var list)) return list;

            if (fileDataHandler.IsFileValid())
            {
                wrongPotions = fileDataHandler.Load().wrongPotions;
            }
            return wrongPotions;
        }

        private bool TryLoadLegacy(out List<WrongPotion> list)
        {
            if (File.Exists(Application.persistentDataPath + WrongRecipeKey))
            {
                string saveData = File.ReadAllText(Application.persistentDataPath + WrongRecipeKey);
                JsonUtility.FromJsonOverwrite(saveData, this);
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