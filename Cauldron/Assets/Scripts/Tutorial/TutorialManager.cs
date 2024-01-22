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

    [Inject]
    private void Construct(RecipeBook book, GameDataHandler dataHandler,
                            Cauldron witchCauldron, GameStateMachine gameStateMachine, MainSettings settings)
    {
        recipeBook = book;
        gameDataHandler = dataHandler;
        cauldron = witchCauldron;
        stateMachine = gameStateMachine;
        recipeHintsStorage = settings.recipeHintsStorage;
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
        if (PlayerPrefs.GetInt(PrefKeys.Tutorial.RECIPE_HINT_ADDED, 0) == 0)
        {
            PlayerPrefs.SetInt(PrefKeys.Tutorial.RECIPE_HINT_ADDED, 1);
            tooltipPrefab.Open(RecipeHintTutorialText).Forget();
        }
    }

    private void ViewBookTutorial()
    {
        recipeBook.OnOpenBook -= ViewBookTutorial;
        
        if (PlayerPrefs.GetInt(PrefKeys.Tutorial.BOOK_OPENED_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(PrefKeys.Tutorial.BOOK_OPENED_KEY, 1);
            tooltipPrefab.Open(BookTutorialText).Forget();
        }
    }

    private void ViewAutoCookingTutorial()
    {
        recipeBook.OnOpenBook -= ViewAutoCookingTutorial;
        
        PlayerPrefs.SetInt(PrefKeys.Tutorial.BOOK_AUTOCOOKING_KEY, 1);
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
            if (PlayerPrefs.GetInt(PrefKeys.Tutorial.VISITOR_KEY, 0) == 0)
            {
                PlayerPrefs.SetInt(PrefKeys.Tutorial.VISITOR_KEY, 1);
                tooltipPrefab.Open(VisitorTutorialText).Forget();
            }
        }
    }

    private void ViewScaleChangeTutorial()
    {
        gameDataHandler.StatusChanged -= ViewScaleChangeTutorial;
        if (PlayerPrefs.GetInt(PrefKeys.Tutorial.SCALE_CHANGE_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(PrefKeys.Tutorial.SCALE_CHANGE_KEY, 1);
            tooltipPrefab.Open(ScaleTutorialText).Forget();
        }
    }

    private void ViewPotionDeniedTutorial()
    {
        if(stateMachine.currentGamePhase == GameStateMachine.GamePhase.Night) return;
        
        cauldron.PotionDeclined -= ViewPotionDeniedTutorial;
        if (PlayerPrefs.GetInt(PrefKeys.Tutorial.POTION_DENIED_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(PrefKeys.Tutorial.POTION_DENIED_KEY, 1);
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