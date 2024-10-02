using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [CreateAssetMenu(order = 50, menuName = "GameModes/AddEncounter")]
    public class AddEncounterGameMode: GameModeBase
    {
        public Encounter cardToAdd;
        
        private EncounterDeck deck;

        [Inject]
        public void Construct(EncounterDeck encounterDeck)
        {
            deck = encounterDeck;
        }
        
        public override void Apply()
        {
            deck.AddToDeck(cardToAdd, true);
        }

        public override bool ShouldReapply => false;
    }
}