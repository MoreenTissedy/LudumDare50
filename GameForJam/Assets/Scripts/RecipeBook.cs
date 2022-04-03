using System;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class RecipeBook : MonoBehaviour
    {
        public static RecipeBook instance;
    
    
        public GameObject bookObject;
        public RecipeBookEntry entry1, entry2, entry3;
        public int entryNumber = 3;
        
        public Recipe[] recipes;

        private int currentRecipe = 0;

        private void Awake()
        {
            if (instance is null)
                instance = this;
            else
            {
                Debug.LogError("double singleton:"+this.GetType().Name);
            }
            
        }

        private void Start()
        {
            CloseBook();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ToggleBook();
            if (Input.GetKeyDown(KeyCode.RightArrow))
                NextPage();
            if (Input.GetKeyDown(KeyCode.LeftArrow))
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
            currentRecipe = 0;
            UpdatePage();
        }

        void UpdatePage()
        {
            //sound
            entry1.Display(recipes[currentRecipe]);
            if (recipes.Length > currentRecipe + 1)
            {
                entry2.Display(recipes[currentRecipe + 1]);
            }
            else
            {
                entry2.Clear();
            }
            if (recipes.Length > currentRecipe + 2)
            {
                entry3.Display(recipes[currentRecipe + 2]);
            }
            else
            {
                entry3.Clear();
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
            if (recipes.Length - 1 < currentRecipe + entryNumber )
                return;
            currentRecipe += entryNumber;
            UpdatePage();
        }

        public void PrevPage()
        {
            if (!bookObject.activeInHierarchy)
                return;
            if (currentRecipe - entryNumber < 0)
                return;
            currentRecipe -= entryNumber;
            UpdatePage();
        }
    }
}