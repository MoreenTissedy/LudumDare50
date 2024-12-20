using CauldronCodebase.GameStates;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    [RequireComponent(typeof(Animator))]
    public class CatTutorial:MonoBehaviour
    {
        [SerializeField] private GameObject catToTheRight, catToTheLeft;
        [SerializeField] private Canvas catDialog;
        [SerializeField] private Animator scheme;

        private Recipe selectedRecipe;
        [Inject]
        private Cauldron pot;
        [Inject]
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

        private bool anyKeyPressed;

        public bool TutorialMode
        {
            get => tutorialMode;
        }

        void OnValidate()
        {
            scheme = GetComponent<Animator>();
            scheme.enabled = false;
        }
        
        public void Start()
        {
            //change scene
            catToTheRight.SetActive(false);
            catToTheLeft.SetActive(true);
            catDialog.enabled = true;
            Show("Привет!");

            //subscribe to cauldron
            pot.IngredientAdded += PotOnIngredientAdded;
            pot.PotionBrewed += PotOnPotionBrewed;
            //subscribe to recipebook
            book.OnSelectRecipe += BookOnOnSelectRecipe;
            
            //start tutorial
            StartCoroutine(StartOnInput());
            //StartCoroutine(RegularInputUpdate());
        }

        IEnumerator StartOnInput()
        {
            yield return new WaitUntil(() => Input.anyKey);
            scheme.enabled = true;
            tutorialMode = true;
        }

        IEnumerator RegularInputUpdate()
        {
            var seconds = new WaitForSeconds(1f);
            while (tutorialMode)
            {
                yield return seconds;
                scheme.SetBool(AnyKey, anyKeyPressed);
                
                Debug.Log(anyKeyPressed);
                anyKeyPressed = false;
            }
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
            scheme.SetInteger(MixCount, pot.Mix.Count);
            bool valid = false;
            if (!(selectedRecipe is null))
            {
                valid = selectedRecipe.RecipeIngredients.Contains(ingredient);
            }

            scheme.SetBool(Right, valid);
        }

        public void End()
        {
            StartCoroutine(EndTutorial());
        }

        private IEnumerator EndTutorial()
        {
            yield return new WaitUntil(()=> Input.anyKeyDown);
            tutorialMode = false;
            scheme.enabled = false;
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
            
            if (!anyKeyPressed && Input.anyKey)
            {
                Debug.Log("set to true");
                anyKeyPressed = true;
            }
        }

        public void Show(string text)
        {
            catDialog.GetComponentInChildren<Text>().text = text;
        }

        public void ClearPot()
        {
            pot.ClearAndActivate(GameStateMachine.GamePhase.Visitor);
        }
    }
}