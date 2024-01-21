using System.Linq;
using UnityEngine;

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

        public VisitorState(EncounterDeck deck,
                            MainSettings settings,
                            GameDataHandler gameDataHandler,
                            VisitorManager visitorManager,
                            Cauldron cauldron,
                            GameStateMachine stateMachine,
                            NightEventProvider nightEventProvider, 
                            SoundManager soundManager,
                            StatusChecker statusChecker, 
                            IAchievementManager achievementManager)
        {
            cardDeck = deck;
            this.gameDataHandler = gameDataHandler;
            this.visitorManager = visitorManager;
            this.cauldron = cauldron;
            this.stateMachine = stateMachine;
            this.soundManager = soundManager;
            this.statusChecker = statusChecker;
            this.achievementManager = achievementManager;

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
        }

        public override void Exit()
        {
            gameDataHandler.cardsDrawnToday++;
            cauldron.PotionAccepted -= EndEncounter;
            visitorManager.VisitorLeft -= OnVisitorLeft;
            visitorManager.Exit();           
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
            gameDataHandler.AddPotion(potion, !resolver.EndEncounter(potion));
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