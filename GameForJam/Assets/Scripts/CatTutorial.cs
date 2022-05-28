using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Animator))]
    public class CatTutorial:MonoBehaviour
    {
        [SerializeField] private GameObject catToTheRight, catToTheLeft;
        [SerializeField] private Canvas catDialog;
        [SerializeField] private Animator scheme;

        private Recipe selectedRecipe;
        private Cauldron pot;
        private RecipeBook book;
        private bool tutorialMode;

        public event Action OnEnd;
        
        private static readonly int SelectRecipe = Animator.StringToHash("SelectRecipe");
        private static readonly int TempRight = Animator.StringToHash("TempRight");
        private static readonly int MixRight = Animator.StringToHash("MixRight");
        private static readonly int PotionBrewed = Animator.StringToHash("PotionBrewed");
        private static readonly int Right = Animator.StringToHash("Right");
        private static readonly int IngredientAdded = Animator.StringToHash("IngredientAdded");
        private static readonly int AnyKey = Animator.StringToHash("AnyKey");
        private static readonly int MixCount = Animator.StringToHash("MixCount");

        public bool TutorialMode
        {
            get => tutorialMode;
        }

        void OnValidate()
        {
            scheme = GetComponent<Animator>();
        }
        
        public void Start()
        {
            //change scene
            catToTheRight.SetActive(false);
            catToTheLeft.SetActive(true);
            catDialog.enabled = true;
            
            pot = Cauldron.instance;
            book = RecipeBook.instance;

            //subscribe to cauldron
            pot.IngredientAdded += PotOnIngredientAdded;
            pot.PotionBrewed += PotOnPotionBrewed;
            //subscribe to recipebook
            book.OnSelectRecipe += BookOnOnSelectRecipe;
            
            //start tutorial
            tutorialMode = true;
        }

        private void BookOnOnSelectRecipe(Recipe obj)
        {
            selectedRecipe = obj;
            scheme.SetTrigger(SelectRecipe);
        }

        private void PotOnPotionBrewed(Potions potion)
        {
            scheme.SetTrigger(PotionBrewed);
            scheme.SetInteger(MixCount, 0);
            scheme.SetBool(Right, selectedRecipe.potion == potion);
        }

        private void PotOnIngredientAdded(Ingredients ingredient)
        {
            scheme.SetTrigger(IngredientAdded);
            scheme.SetInteger(MixCount, pot.mix.Count);
            bool valid = false;
            if (!(selectedRecipe is null))
            {
                valid = selectedRecipe.ingredient1 == ingredient ||
                             selectedRecipe.ingredient2 == ingredient ||
                             selectedRecipe.ingredient3 == ingredient;
            }

            scheme.SetBool(Right, valid);
        }

        public void End()
        {
            tutorialMode = false;
            catToTheLeft.SetActive(false);
            catToTheRight.SetActive(true);
            catDialog.enabled = false;
            pot.IngredientAdded -= PotOnIngredientAdded;
            pot.PotionBrewed -= PotOnPotionBrewed;
            book.OnSelectRecipe -= BookOnOnSelectRecipe;
            OnEnd?.Invoke();
        }

        void Update()
        {
            if (!tutorialMode)
                return;
            
            //fire, mixProcess
            scheme.SetBool(TempRight, pot.IsBoiling);
            scheme.SetBool(MixRight, pot.IsMixRight);
            
            // //any key
            // if (Input.anyKeyDown)
            // {
            //     scheme.SetBool(AnyKey, true);
            // }
            // else
            // {
            //     scheme.SetBool(AnyKey, false);
            // }
        }

        public void Show(string text)
        {
            catDialog.GetComponentInChildren<Text>().text = text;
        }
    }
}