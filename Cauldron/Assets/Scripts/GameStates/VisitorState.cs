using System.Linq;
using UnityEngine.InputSystem.UI;

namespace CauldronCodebase.GameStates
{
    public class VisitorState : BaseGameState
    {
        private readonly EncounterDeck cardDeck;
        private readonly GameDataHandler gameDataHandler;
        private readonly VisitorManager visitorManager;
        private readonly Cauldron cauldron;
        private readonly GameStateMachine stateMachine;
        private readonly SoundManager soundManager;

        private readonly EncounterResolver resolver;
        private readonly StatusChecker statusChecker;
        private readonly IAchievementManager achievementManager;
        private readonly InputManager inputManager;

        public VisitorState(EncounterDeck deck,
                            MainSettings settings,
                            GameDataHandler gameDataHandler,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            GameStateMachine stateMachine,
                            NightEventProvider nightEventProvider, 
                            SoundManager soundManager,
                            StatusChecker statusChecker, 
                            IAchievementManager achievementManager, 
                            InputManager input)
        {
            cardDeck = deck;
            this.gameDataHandler = gameDataHandler;
            this.visitorManager = visitorManager;
            this.cauldron = cauldron;
            this.stateMachine = stateMachine;
            this.soundManager = soundManager;
            this.statusChecker = statusChecker;
            this.achievementManager = achievementManager;
            this.inputManager = input;

            resolver = new EncounterResolver(settings, gameDataHandler, deck, nightEventProvider);
        }
        
        public override void Enter()
        {
            var priorityCard = statusChecker.CheckStatusesThreshold();
            if (priorityCard)
            {
                cardDeck.AddToDeck(priorityCard, true);
            }
            Encounter currentCard = cardDeck.GetTopCard();
            if(currentCard is null) return;
            
            gameDataHandler.SetCurrentCard(currentCard);
                     
            visitorManager.Enter(currentCard);
            cauldron.PotionAccepted += EndEncounter;
            visitorManager.VisitorLeft += OnVisitorLeft;

            inputManager.SetCursor(true);
        }

        public override void Exit()
        {
            gameDataHandler.cardsDrawnToday++;
            cauldron.PotionAccepted -= EndEncounter;
            visitorManager.VisitorLeft -= OnVisitorLeft;
            visitorManager.Exit();    
            inputManager.SetCursor(false);
        }

        private void OnVisitorLeft()
        {
            EndEncounter(Potions.Placebo);
        }

        private void EndEncounter(Potions potion)
        {
            if (!EncounterIdents.GetAllSpecialCharacters().Contains(gameDataHandler.currentCard.villager.name))
            {
                achievementManager.TryUnlock(AchievIdents.FIRST_POTION);
            }
            SignalPotionSuccess(potion);
            bool potionEffective = resolver.EndEncounter(potion);
            gameDataHandler.AddPotion(potion, !potionEffective);
            stateMachine.SwitchState(GameStateMachine.GamePhase.VisitorWaiting);
        }

        private void SignalPotionSuccess(Potions potion)
        {
            var potionCoef = gameDataHandler.currentCard.resultsByPotion.FirstOrDefault(x => x.potion == potion)?.influenceCoef ?? 0;
            if (potionCoef > 0)
            {
                soundManager.Play(Sounds.PotionSuccess);
                visitorManager.PlayReaction(true);
            }
            else if (potionCoef < 0)
            {
                soundManager.Play(Sounds.PotionFailure);
                visitorManager.PlayReaction(false);
            }
        }
    }
}