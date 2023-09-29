using System.Collections.Generic;

namespace CauldronCodebase
{
    public static class EncounterIdents
    {
        public const string INQUISITION = "Inquisition";
        public const string DARK_STRANGER = "DarkStranger";
        public const string WITCH_MEMORY = "WitchMemory";
        public const string CAT = "Cat";
        
        public const string CAT_UNLOCK = "intro-4";
        
        public static HashSet<string> GetAllSpecialCharacters()
        {
            return new HashSet<string> { INQUISITION, DARK_STRANGER, WITCH_MEMORY, CAT};
        }
    }
}