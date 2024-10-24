﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "WrongRecipeProvider", menuName = "WrongRecipeProvider")]
    public class WrongRecipeProvider : ScriptableObject
    {
        private const string WrongRecipeKey = "WrongRecipe";
        
        public List<WrongPotion> wrongPotions = new List<WrongPotion>();

        public void ResetWrongRecipe()
        {
            wrongPotions.Clear();
            SaveWrongRecipe();
        }

        public void SaveWrongRecipe()
        {
            string saveData = JsonUtility.ToJson(this);
            File.WriteAllText(Application.persistentDataPath + WrongRecipeKey, saveData);
        }

        public List<WrongPotion> LoadWrongRecipe()
        {
            if (File.Exists(Application.persistentDataPath + WrongRecipeKey))
            {
                string saveData = File.ReadAllText(Application.persistentDataPath + WrongRecipeKey);
                try
                {
                    JsonUtility.FromJsonOverwrite(saveData, this);
                }
                catch (Exception e)
                {
                    Debug.LogError("[JSON Error] Wrong recipe file corrupted! Delete it.");
                    File.Delete(Application.persistentDataPath + WrongRecipeKey);
                }
            }

            return wrongPotions;
        }
    }
}