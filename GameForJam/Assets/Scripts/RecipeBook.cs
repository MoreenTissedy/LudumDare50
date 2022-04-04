using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class RecipeBook : MonoBehaviour
    {
        public static RecipeBook instance;
    
    
        public GameObject bookObject;
        public RecipeBookEntry[] entries;
        public GameObject rightCorner, leftCorner; 
        
        public Recipe[] recipes;

        private int currentRecipe = 0;

        private void Awake()
        {
            // if (instance is null)
            //     instance = this;
            // else
            // {
            //     Debug.LogError("double singleton:"+this.GetType().Name);
            // }
            instance = this;
            CloseBook();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ToggleBook();
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                NextPage();
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                PrevPage();
        }

        public Recipe GetRecipeForPotion(Potions potion)
        {
            var found = recipes.Where(x => x.potion == potion).ToArray();
            if (found.Length > 0)
            {
                return found[0];
            }
            else
            {
                return null;
            }
        }

        void ToggleBook()
        {
            if (bookObject.activeInHierarchy)
                CloseBook();
            else
            {
                OpenBook();
            }
        }

        public void OpenBook()
        {
            bookObject.SetActive(true);
            StartCoroutine(UpdateWithDelay());
        }

        IEnumerator UpdateWithDelay()
        {
            yield return null;
            UpdatePage();
        }

        void UpdatePage()
        {
            //sound
            for (int i = 0; i < entries.Length; i++)
            {
                int num = currentRecipe + i;
                if (num < recipes.Length)
                {
                    entries[i].Display(recipes[num]);
                }
                else
                {
                    entries[i].Clear();
                }
            }
        }

        public void CloseBook()
        {
            //sound
            bookObject.SetActive(false);
        }

        public void NextPage()
        {
            if (!bookObject.activeInHierarchy)
                return;
            if (recipes.Length - 1 < currentRecipe + entries.Length )
                return;
            currentRecipe += entries.Length;
            leftCorner.SetActive(true);
            rightCorner.SetActive(false);
            UpdatePage();
        }

        public void PrevPage()
        {
            if (!bookObject.activeInHierarchy)
                return;
            if (currentRecipe - entries.Length < 0)
                return;
            currentRecipe -= entries.Length;
            leftCorner.SetActive(false);
            rightCorner.SetActive(true);
            UpdatePage();
        }
    }
}