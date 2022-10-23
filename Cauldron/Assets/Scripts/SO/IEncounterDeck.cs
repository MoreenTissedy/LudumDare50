namespace CauldronCodebase
{
    public interface IEncounterDeck
    {
        void Init(GameState game);
        void NewDayPool(int day);

        /// <summary>
        /// Add X random cards from pool to deck
        /// </summary>
        /// <param name="num">X - number of cards</param>
        void DealCards(int num);

        void AddCardToPool(Encounter card);
        void AddToDeck(Encounter card, bool asFirst = false);
        Encounter GetTopCard();
    }
}