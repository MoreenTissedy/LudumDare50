using System.Collections.Generic;

namespace CauldronCodebase
{
    public static class PrefKeys
    {
        public const string UniqueCards = "RememberedCards";  //not needed in the cloud
        
        //TODO cloud
        public const string UnlockedRecipes = "Recipes";
        public const string CurrentRound = "CurrentRound";
        public const string UnlockedEndings = "Endings";
        
        public const string RecipeHints = "RecipeHints";  //converted to cloud save - used for backwards compatibility
        
        //TODO cloud
        public const string Milestones = "Milestones";
        public const string CovenIntroShown = "CovenIntro";
        public const string IsAutoCookingUnlocked = "IsAutoCookingUnlocked";
        
        //settings - not needed in the cloud
        public const string MusicValueSettings = "MusicValue";
        public const string SoundsValueSettings = "SoundsValue";
        public const string ResolutionSettings = "ResolutionSettings";
        public const string FullscreenModeSettings = "SetFullscreenSettings";
        public const string AutoCooking = "AutoCooking";
        public const string LanguageKey = "LanguageSettings";

        public class PlayerProgress
        {
            public List<string> UnlockedRecipes;
            public List<string> UnlockedEndings;
            public List<string> Milestones;
            public int CurrentRound;
            public bool CovenIntroShown;
            public bool IsAutoCookingUnlocked;
        }
    }
}