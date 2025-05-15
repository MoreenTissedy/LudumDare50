using System;
using CauldronCodebase;
using System.Collections.Generic;
using System.Threading;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using EasyLoc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Universal;
using Zenject;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialScreen tutorialScreen;

    [Localize] [TextArea (5, 10)] public string BookTutorialText;
    [Localize] [TextArea (5, 10)] public string BookTutorialGamepadControls;
    [Localize] [TextArea (5, 10)] public string BookTutorialKeyboardControls;
    [Localize] [TextArea (5, 10)] public string VisitorTutorialText;
    [Localize] [TextArea (5, 10)] public string ScaleTutorialText;
    [Localize] [TextArea (5, 10)] public string PotionDeniedTutorialText;
    [Localize] [TextArea (5, 10)] public string DescriptionTutorialAutoCooking;
    [Localize] [TextArea(5, 10)] public string RecipeHintTutorialText;
    [Localize] [TextArea(5, 10)] public string WardrobeTutorialText;
    [Localize] [TextArea(5, 10)] public string PremiumTutorialText;

    [SerializeField] private Encounter targetTutorialVisitor;
    
    private RecipeBook recipeBook;
    private GameDataHandler gameDataHandler;
    private Cauldron cauldron;
    private GameStateMachine stateMachine;
    private RecipeHintsStorage recipeHintsStorage;
    private TutorialStorage tutorialStorage;
    private Wardrobe wardrobe;
    private OverlayManager overlayManager;
    
    private HashSet<TutorialKeys> tutorials;
    private CancellationTokenSource cts = new CancellationTokenSource();

    [Inject]
    private void Construct(RecipeBook book, GameDataHandler dataHandler,
                            Cauldron witchCauldron, GameStateMachine gameStateMachine, MainSettings settings, Wardrobe wardrobe, OverlayManager overlayManager)
    {
        recipeBook = book;
        gameDataHandler = dataHandler;
        cauldron = witchCauldron;
        stateMachine = gameStateMachine;
        recipeHintsStorage = settings.recipeHintsStorage;
        this.wardrobe = wardrobe;
        this.overlayManager = overlayManager;
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

        if (!tutorials.Contains(TutorialKeys.TUTORIAL_WARDROBE))
        {
            wardrobe.OnApplyCondition += OnWardrobeApply;
        }

        TryShowTutorialForPremium();
    }

    private async void TryShowTutorialForPremium()
    {
        if (tutorials.Contains(TutorialKeys.TUTORIAL_PREMIUM))
        {
            return;
        }
        if (await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: cts.Token).SuppressCancellationThrow())
        {
            return;
        }
        if (SteamConnector.HasPremium)
        {
            SaveKey(TutorialKeys.TUTORIAL_PREMIUM);
            tutorialScreen.Show(PremiumTutorialText).Forget();
        }
    }

    private async UniTask<bool> OnWardrobeApply()
    {
        var result = await tutorialScreen.ShowAsDialog(WardrobeTutorialText);
        SaveKey(TutorialKeys.TUTORIAL_WARDROBE);
        wardrobe.OnApplyCondition -= OnWardrobeApply;
        return result;
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
            tutorialScreen.Show(RecipeHintTutorialText).Forget();
        }
    }

    private void ViewBookTutorial()
    {
        recipeBook.OnOpenBook -= ViewBookTutorial;
        
        if(!tutorials.Contains(TutorialKeys.TUTORIAL_BOOK_OPENED))
        {
            SaveKey(TutorialKeys.TUTORIAL_BOOK_OPENED);
            string tutorialText = string.Format(BookTutorialText,
                Gamepad.current != null ? BookTutorialGamepadControls : BookTutorialKeyboardControls);
            tutorialScreen.Show(tutorialText).Forget();
        }
    }

    private async void ViewAutoCookingTutorial()
    {
        recipeBook.OnOpenBook -= ViewAutoCookingTutorial;
        
        SaveKey(TutorialKeys.BOOK_AUTOCOOKING_OPENED);
        bool accepted = await tutorialScreen.ShowAsDialog(DescriptionTutorialAutoCooking);
        PlayerPrefs.SetInt(PrefKeys.AutoCooking, accepted ? 1 : 0);
    }

    private void ViewVisitorTutorial(Potions potion)
    {
        if (gameDataHandler.currentCard == targetTutorialVisitor)
        {
            cauldron.PotionAccepted -= ViewVisitorTutorial;
            
            if(!tutorials.Contains(TutorialKeys.TUTORIAL_VISITOR))
            {
                SaveKey(TutorialKeys.TUTORIAL_VISITOR);
                tutorialScreen.Show(VisitorTutorialText).Forget();
            }
        }
    }

    private void ViewScaleChangeTutorial(Statustype statustype, int i)
    {
        gameDataHandler.StatusChanged -= ViewScaleChangeTutorial;
        
        if(!tutorials.Contains(TutorialKeys.TUTORIAL_CHANGE_SCALE))
        {            
            SaveKey(TutorialKeys.TUTORIAL_CHANGE_SCALE);
            tutorialScreen.Show(ScaleTutorialText).Forget();
        }
    }

    private void ViewPotionDeniedTutorial()
    {
        if(stateMachine.currentGamePhase == GameStateMachine.GamePhase.Night) return;
        
        cauldron.PotionDeclined -= ViewPotionDeniedTutorial;
        
        if(!tutorials.Contains(TutorialKeys.TUTORIAL_POTION_DENIED))
        {
            SaveKey(TutorialKeys.TUTORIAL_POTION_DENIED);
            tutorialScreen.Show(PotionDeniedTutorialText).Forget();
        }
    }

    private void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
}