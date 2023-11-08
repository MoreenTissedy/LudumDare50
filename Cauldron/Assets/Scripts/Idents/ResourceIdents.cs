using System.Collections.Generic;

namespace CauldronCodebase
{
    public static class ResourceIdents
    {
        public static readonly string[] Backgrounds = new string[]
        {
            "Backgrounds/Basic",
            "Backgrounds/Forest",
           // "Backgrounds/Swamp",
            "Backgrounds/Field"
        };
        public const string EndingScreen = "EndingPanel";

        public static readonly Dictionary<string, string> EndingCartoons = new Dictionary<string, string>()
        {
            {EndingsProvider.ENOUGH_MONEY, "Endings/Coven"}
        };
    }
}