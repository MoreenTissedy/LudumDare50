using UnityEngine;

namespace CauldronCodebase
{
    //This class is only to display interface in Unity inspector
    public abstract class EncounterDeckBase : ScriptableObject, IEncounterDeck
    {
        public abstract void Init(GameData game);

        public abstract void NewDayPool(int day);

        public abstract void DealCards(int num);

        public abstract void AddCardToPool(Encounter card);

        public abstract void AddToDeck(Encounter card, bool asFirst = false);

        public abstract Encounter GetTopCard();
    }
}