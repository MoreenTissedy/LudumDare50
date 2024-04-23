using System.Collections.Generic;

namespace CauldronCodebase
{
    public static class ResourceIdents
    {
        public static readonly string[] Backgrounds = new string[]
        {
            "Backgrounds/Basic",
            "Backgrounds/Forest",
            "Backgrounds/Swamp",
            "Backgrounds/Field"
        };
        public const string EndingScreen = "EndingPanel";

        public static readonly Dictionary<string, string> EndingCartoons = new Dictionary<string, string>()
        {
            {EndingsProvider.ENOUGH_MONEY, "Endings/Coven"},
            {EndingsProvider.HIGH_FAME, "Endings/Fire"},
            {EndingsProvider.HIGH_FEAR, "Endings/Mob"},
            {EndingsProvider.LOW_FAME, "Endings/Castaway"},
            {EndingsProvider.LOW_FEAR, "Endings/Robbery"},
            {EndingsProvider.END_DECK, "Endings/Moving"},
            {EndingsProvider.KING_GOOD, "Endings/RoyalWitch"},
            {EndingsProvider.KING_BAD, "Endings/RaceForPower"},
            {EndingsProvider.BANDIT, "Endings/Revolt"},
            {EndingsProvider.BISHOP_GOOD, "Endings/TruePower"},
            {EndingsProvider.BISHOP_BAD, "Endings/Sackcloth"},
            {EndingsProvider.FINAL, "Endings/Freedom"},
        };
    }
}