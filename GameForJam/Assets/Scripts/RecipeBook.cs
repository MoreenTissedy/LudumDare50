using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class RecipeBook : MonoBehaviour
    {
        public static RecipeBook instance;
    
    
        public GameObject bookObject;
        public RecipeBookEntry[] entries;
        public GameObject rightCorner, leftCorner; 
        
        public Recipe[] recipes;

        private int currentPage = 0;

        public AudioSource left, right;
        public Text prevPageNum, nextPageNum;
        

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
            leftCorner.SetActive(false);
        }

        [ContextMenu("Export Recipes to CSV")]
        public void ExportRecipes()
        {
            var file = File.CreateText(Application.dataPath+"/Localize/Recipes.csv");
            file.WriteLine("id;name_RU;description_RU;name_EN;description_EN");
            foreach (var recipe in recipes)
            {
                file.WriteLine(recipe.name+";"+recipe.potionName+";"+recipe.description);
            }
            file.Close();
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
            left.Play();
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
                int num = currentPage*entries.Length + i;
                if (num < recipes.Length)
                {
                    entries[i].Display(recipes[num]);
                }
                else
                {
                    entries[i].Clear();
                }
            }
            nextPageNum.text = (currentPage *2+2).ToString();
            prevPageNum.text = (currentPage*2+1).ToString();
        }

        public void CloseBook()
        {
            right.Play();
            bookObject.SetActive(false);
        }

        public void NextPage()
        {
            if (!bookObject.activeInHierarchy)
                return;
            if ((currentPage+1)*entries.Length >= recipes.Length )
                return;
            currentPage++;
            if ((currentPage+1)*entries.Length >= recipes.Length )
                rightCorner.SetActive(false);
            leftCorner.SetActive(true);
            UpdatePage();
            right.Play();
        }

        public void PrevPage()
        {
            if (!bookObject.activeInHierarchy)
                return;
            if (currentPage <= 0)
                return;
            currentPage--;
            if (currentPage == 0)
                leftCorner.SetActive(false);
            rightCorner.SetActive(true);
            UpdatePage();
            left.Play();
        }
    }
}