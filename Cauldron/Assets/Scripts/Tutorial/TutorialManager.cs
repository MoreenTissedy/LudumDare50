using CauldronCodebase;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using EasyLoc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Zenject;

public class TutorialManager : MonoBehaviour
{
    private const string BOOK_OPENED_KEY = "TUTORIAL_BOOK_OPENED";
    private const string BOOK_AUTOCOOKING_KEY = "TUTORIAL_BOOK_OPENED";
    private const string VISITOR_KEY = "TUTORIAL_VISITOR";
    private const string SCALE_CHANGE_KEY = "TUTORIAL_CHANGE_SCALE";
    private const string POTION_DENIED_KEY = "TUTORIAL_POTION_DENIED";

    [SerializeField] private ScrollTooltip tooltipPrefab;

    [Localize] [TextArea (5, 10)] public string BookTutorialText;
    [Localize] [TextArea (5, 10)] public string VisitorTutorialText;
    [Localize] [TextArea (5, 10)] public string ScaleTutorialText;
    [Localize] [TextArea (5, 10)] public string PotionDeniedTutorialText;
    [Localize] [TextArea (5, 10)] public string DescriptionTutorialAutoCooking;

    [SerializeField] private Encounter targetTutorialVisitor;
    [SerializeField] private Button rejectButton;
    [SerializeField] private Button acceptButton;
    
    private RecipeBook recipeBook;
    private GameDataHandler gameDataHandler;
    private Cauldron cauldron;
    private GameStateMachine stateMachine;

    [Inject]
    private void Construct(RecipeBook book, GameDataHandler dataHandler,
                            Cauldron witchCauldron, GameStateMachine gameStateMachine)
    {
        recipeBook = book;
        gameDataHandler = dataHandler;
        cauldron = witchCauldron;
        stateMachine = gameStateMachine;
    }

    private void Start()
    {
        recipeBook.OnOpenBook += ViewBookTutorial;
        recipeBook.OnOpenAutoCooking += ViewAutoCookingTutorial;
        cauldron.PotionAccepted += ViewVisitorTutorial;
        gameDataHandler.StatusChanged += ViewScaleChangeTutorial;
        cauldron.PotionDeclined += ViewPotionDeniedTutorial;
    }

    private void ViewBookTutorial()
    {
        recipeBook.OnOpenBook -= ViewBookTutorial;
        
        if (PlayerPrefs.GetInt(BOOK_OPENED_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(BOOK_OPENED_KEY, 1);
            tooltipPrefab.Open(BookTutorialText).Forget();
        }
    }

    private void ViewAutoCookingTutorial()
    {
        recipeBook.OnOpenBook -= ViewAutoCookingTutorial;
        
        if (PlayerPrefs.GetInt(BOOK_AUTOCOOKING_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(BOOK_AUTOCOOKING_KEY, 1);
            acceptButton.onClick.AddListener(AcceptAutoCookingClickButton);
            rejectButton.onClick.AddListener(RejectAutoCookingClickButton);
            tooltipPrefab.Open(DescriptionTutorialAutoCooking).Forget();
            acceptButton.gameObject.SetActive(true);
            rejectButton.gameObject.SetActive(true);
        }
    }

    private void ViewVisitorTutorial(Potions potion)
    {
        if (gameDataHandler.currentCard == targetTutorialVisitor)
        {
            cauldron.PotionAccepted -= ViewVisitorTutorial;
            if (PlayerPrefs.GetInt(VISITOR_KEY, 0) == 0)
            {
                PlayerPrefs.SetInt(VISITOR_KEY, 1);
                tooltipPrefab.Open(VisitorTutorialText).Forget();
            }
        }
    }

    private void ViewScaleChangeTutorial()
    {
        gameDataHandler.StatusChanged -= ViewScaleChangeTutorial;
        if (PlayerPrefs.GetInt(SCALE_CHANGE_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(SCALE_CHANGE_KEY, 1);
            tooltipPrefab.Open(ScaleTutorialText).Forget();
        }
    }

    private void ViewPotionDeniedTutorial()
    {
        if(stateMachine.currentGamePhase == GameStateMachine.GamePhase.Night) return;
        
        cauldron.PotionDeclined -= ViewPotionDeniedTutorial;
        if (PlayerPrefs.GetInt(POTION_DENIED_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(POTION_DENIED_KEY, 1);
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