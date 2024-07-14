using CauldronCodebase;
using System.Collections.Generic;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using EasyLoc;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Zenject;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private ScrollTooltip tooltipPrefab;

    [Localize] [TextArea (5, 10)] public string BookTutorialText;
    [Localize] [TextArea (5, 10)] public string VisitorTutorialText;
    [Localize] [TextArea (5, 10)] public string ScaleTutorialText;
    [Localize] [TextArea (5, 10)] public string PotionDeniedTutorialText;
    [Localize] [TextArea (5, 10)] public string DescriptionTutorialAutoCooking;
    [Localize] [TextArea(5, 10)] public string RecipeHintTutorialText;

    [SerializeField] private Encounter targetTutorialVisitor;
    [SerializeField] private Button rejectButton;
    [SerializeField] private Button acceptButton;
    
    private RecipeBook recipeBook;
    private GameDataHandler gameDataHandler;
    private Cauldron cauldron;
    private GameStateMachine stateMachine;
    private RecipeHintsStorage recipeHintsStorage;
    private TutorialStorage tutorialStorage;
    private HashSet<TutorialKeys> tutorials;

    [Inject]
    private void Construct(RecipeBook book, GameDataHandler dataHandler,
                            Cauldron witchCauldron, GameStateMachine gameStateMachine, MainSettings settings)
    {
        recipeBook = book;
        gameDataHandler = dataHandler;
        cauldron = witchCauldron;
        stateMachine = gameStateMachine;
        recipeHintsStorage = settings.recipeHintsStorage;
        tutorialStorage = new TutorialStorage();
    }

    private void Start()
    {
        tutorials = tutorialStorage.GetTutorials();
        
        recipeBook.OnOpenBook += ViewBookTutorial;
        recipeBook.OnUnlockAutoCooking += ViewAutoCookingTutorial;
        cauldron.PotionAccepted += ViewVisitorTutorial;
        gameDataHandler.StatusChanged += ViewScaleChangeTutorial;
        cauldron.PotionDeclined += ViewPotionDeniedTutorial;
        recipeHintsStorage.HintAdded += ViewRecipeHintTutorial;
    }

    private void SaveKey(TutorialKeys key)
    {
        tutorials.Add(key);
        tutorialStorage.SaveTutorial(key);
    }

    private void ViewRecipeHintTutorial(RecipeHint hint)
    {
        recipeHintsStorage.HintAdded -= ViewRecipeHintTutorial;
       
        if(!tutorials.Contains(TutorialKeys.TUTORIAL_RECIPE_HINTS))
        {
            SaveKey(TutorialKeys.TUTORIAL_RECIPE_HINTS);
            tooltipPrefab.Open(RecipeHintTutorialText).Forget();
        }
    }

    private void ViewBookTutorial()
    {
        recipeBook.OnOpenBook -= ViewBookTutorial;
        
        if(!tutorials.Contains(TutorialKeys.TUTORIAL_BOOK_OPENED))
        {
            SaveKey(TutorialKeys.TUTORIAL_BOOK_OPENED);
            tooltipPrefab.Open(BookTutorialText).Forget();
        }
    }

    private void ViewAutoCookingTutorial()
    {
        recipeBook.OnOpenBook -= ViewAutoCookingTutorial;
        
        SaveKey(TutorialKeys.BOOK_AUTOCOOKING_OPENED);
        acceptButton.onClick.AddListener(AcceptAutoCookingClickButton);
        rejectButton.onClick.AddListener(RejectAutoCookingClickButton);
        tooltipPrefab.Open(DescriptionTutorialAutoCooking).Forget();
        acceptButton.gameObject.SetActive(true);
        rejectButton.gameObject.SetActive(true);
    }

    private void ViewVisitorTutorial(Potions potion)
    {
        if (gameDataHandler.currentCard == targetTutorialVisitor)
        {
            cauldron.PotionAccepted -= ViewVisitorTutorial;
            
            if(!tutorials.Contains(TutorialKeys.TUTORIAL_VISITOR))
            {
                SaveKey(TutorialKeys.TUTORIAL_VISITOR);
                tooltipPrefab.Open(VisitorTutorialText).Forget();
            }
        }
    }

    private void ViewScaleChangeTutorial()
    {
        gameDataHandler.StatusChanged -= ViewScaleChangeTutorial;
        
        if(!tutorials.Contains(TutorialKeys.TUTORIAL_CHANGE_SCALE))
        {            
            SaveKey(TutorialKeys.TUTORIAL_CHANGE_SCALE);
            tooltipPrefab.Open(ScaleTutorialText).Forget();
        }
    }

    private void ViewPotionDeniedTutorial()
    {
        if(stateMachine.currentGamePhase == GameStateMachine.GamePhase.Night) return;
        
        cauldron.PotionDeclined -= ViewPotionDeniedTutorial;
        
        if(!tutorials.Contains(TutorialKeys.TUTORIAL_POTION_DENIED))
        {
            SaveKey(TutorialKeys.TUTORIAL_POTION_DENIED);
            tooltipPrefab.Open(PotionDeniedTutorialText).Forget();
        }
    }
    
    private void AcceptAutoCookingClickButton()
    {
        PlayerPrefs.SetInt(PrefKeys.AutoCooking, 1);
        DisableButtonAutoCooking();
    }

    private void RejectAutoCookingClickButton()
    {
        PlayerPrefs.SetInt(PrefKeys.AutoCooking, 0);
        DisableButtonAutoCooking();
    }

    private void DisableButtonAutoCooking()
    {
        acceptButton.onClick.RemoveListener(AcceptAutoCookingClickButton);
        rejectButton.onClick.RemoveListener(RejectAutoCookingClickButton);
        acceptButton.gameObject.SetActive(false);
        rejectButton.gameObject.SetActive(false);
    }
}