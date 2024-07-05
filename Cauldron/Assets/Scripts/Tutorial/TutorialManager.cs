using CauldronCodebase;
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

    [Inject]
    private void Construct(RecipeBook book, GameDataHandler dataHandler,
                            Cauldron witchCauldron, GameStateMachine gameStateMachine, MainSettings settings)
    {
        recipeBook = book;
        gameDataHandler = dataHandler;
        cauldron = witchCauldron;
        stateMachine = gameStateMachine;
        recipeHintsStorage = settings.recipeHintsStorage;
        tutorialStorage = settings.tutorialStorage;
    }

    private void Start()
    {
        recipeBook.OnOpenBook += ViewBookTutorial;
        recipeBook.OnUnlockAutoCooking += ViewAutoCookingTutorial;
        cauldron.PotionAccepted += ViewVisitorTutorial;
        gameDataHandler.StatusChanged += ViewScaleChangeTutorial;
        cauldron.PotionDeclined += ViewPotionDeniedTutorial;
        recipeHintsStorage.HintAdded += ViewRecipeHintTutorial;
    }

    private void ViewRecipeHintTutorial(RecipeHint hint)
    {
        recipeHintsStorage.HintAdded -= ViewRecipeHintTutorial;
        tutorialStorage.TryGetTutorial(TutorialKeys.TUTORIAL_RECIPE_HINTS, out var value);
        if(!value)
        {
            tutorialStorage.SaveTutorial(TutorialKeys.TUTORIAL_RECIPE_HINTS, true);
            tooltipPrefab.Open(RecipeHintTutorialText).Forget();
        }
    }

    private void ViewBookTutorial()
    {
        recipeBook.OnOpenBook -= ViewBookTutorial;
        tutorialStorage.TryGetTutorial(TutorialKeys.TUTORIAL_BOOK_OPENED, out var value);
        if(!value)
        {
            tutorialStorage.SaveTutorial(TutorialKeys.TUTORIAL_BOOK_OPENED, true);
            tooltipPrefab.Open(BookTutorialText).Forget();
        }
    }

    private void ViewAutoCookingTutorial()
    {
        recipeBook.OnOpenBook -= ViewAutoCookingTutorial;
        
        tutorialStorage.SaveTutorial(TutorialKeys.BOOK_AUTOCOOKING_OPENED, true);
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
            tutorialStorage.TryGetTutorial(TutorialKeys.TUTORIAL_VISITOR, out var value);
            if(!value)
            {
                tutorialStorage.SaveTutorial(TutorialKeys.TUTORIAL_VISITOR, true);
                tooltipPrefab.Open(VisitorTutorialText).Forget();
            }
        }
    }

    private void ViewScaleChangeTutorial()
    {
        gameDataHandler.StatusChanged -= ViewScaleChangeTutorial;
        tutorialStorage.TryGetTutorial(TutorialKeys.TUTORIAL_CHANGE_SCALE, out var value);
        if(!value)
        {
            tutorialStorage.SaveTutorial(TutorialKeys.TUTORIAL_CHANGE_SCALE, true);
            tooltipPrefab.Open(ScaleTutorialText).Forget();
        }
    }

    private void ViewPotionDeniedTutorial()
    {
        if(stateMachine.currentGamePhase == GameStateMachine.GamePhase.Night) return;
        
        cauldron.PotionDeclined -= ViewPotionDeniedTutorial;
        tutorialStorage.TryGetTutorial(TutorialKeys.TUTORIAL_POTION_DENIED, out var value);
        if(!value)
        {
            tutorialStorage.SaveTutorial(TutorialKeys.TUTORIAL_POTION_DENIED, true);
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