using System;
using System.Collections;
using System.Collections.Generic;
using CauldronCodebase;
using CauldronCodebase.GameStates;
using Cysharp.Threading.Tasks;
using EasyLoc;
using UnityEngine;
using Universal;
using Zenject;

public class TutorialManager : MonoBehaviour
{
    private const string BOOK_OPENED_KEY = "TUTORIAL_BOOK_OPENED";
    private const string VISITOR_KEY = "TUTORIAL_VISITOR";
    private const string SCALE_CHANGE_KEY = "TUTORIAL_CHANGE_SCALE";
    private const string POTION_DENIED_KEY = "TUTORIAL_POTION_DENIED";

    [SerializeField] private ScrollTooltip tooltipPrefab;

    [Localize] public string BookTutorialText;
    [Localize] public string VisitorTutorialText;
    [Localize] public string ScaleTutorialText;
    [Localize] public string PotionDeniedTutorialText;

    [SerializeField] private Encounter targetTutorialVisitor;
    
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
        gameDataHandler.OnNewCard += ViewVisitorTutorial;
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

    private void ViewVisitorTutorial(Encounter encounter)
    {
        Debug.LogWarning(encounter);
        if (encounter == targetTutorialVisitor)
        {
            gameDataHandler.OnNewCard -= ViewVisitorTutorial;
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
            PlayerPrefs.GetInt(SCALE_CHANGE_KEY, 1);
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
}
