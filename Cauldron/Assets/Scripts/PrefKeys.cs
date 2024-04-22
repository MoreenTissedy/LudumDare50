namespace CauldronCodebase
{
    public static class PrefKeys
    {
        public const string UniqueCards = "RememberedCards";  //maybe not needed in the cloud
        
        //converted to cloud save - used for backwards compatibility
        public const string UnlockedRecipes = "Recipes";
        public const string CurrentRound = "CurrentRound";
        public const string UnlockedEndings = "Endings";
        
        public const string RecipeHints = "RecipeHints";  
        
        public const string Milestones = "Milestones";
        public const string CovenIntroShown = "CovenIntro";
        public const string IsAutoCookingUnlocked = "IsAutoCookingUnlocked";
        
        //settings - not needed
        public const string MusicValueSettings = "MusicValue";
        public const string SoundsValueSettings = "SoundsValue";
        public const string ResolutionSettings = "ResolutionSettings";
        public const string FullscreenModeSettings = "SetFullscreenSettings";
        public const string AutoCooking = "AutoCooking";
        public const string LanguageKey = "LanguageSettings";

        public struct Tutorial
        {
            public const string BOOK_OPENED_KEY = "TUTORIAL_BOOK_OPENED";
            public const string BOOK_AUTOCOOKING_KEY = "BOOK_AUTOCOOKING_OPENED";
            public const string VISITOR_KEY = "TUTORIAL_VISITOR";
            public const string SCALE_CHANGE_KEY = "TUTORIAL_CHANGE_SCALE";
            public const string POTION_DENIED_KEY = "TUTORIAL_POTION_DENIED";
            public const string RECIPE_HINT_ADDED = "TUTORIAL_RECIPE_HINTS";
        }
    }
}