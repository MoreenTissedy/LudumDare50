namespace CauldronCodebase
{
    public static class PrefKeys
    {
        //Player flow modifiers - not needed in the cloud
        public const string UniqueCards = "RememberedCards";
        public const string Freezes = "Cooldowns";
        
        //Legacy progress load keys
        public const string UnlockedRecipes = "Recipes";
        public const string CurrentRound = "CurrentRound";
        public const string UnlockedEndings = "Endings";
        public const string RecipeHints = "RecipeHints";
        public const string Milestones = "Milestones";
        public const string CovenIntroShown = "CovenIntro";

        //settings - not needed in the cloud
        public const string MusicValueSettings = "MusicValue";
        public const string SoundsValueSettings = "SoundsValue";
        public const string FullscreenModeSettings = "SetFullscreenSettings";
        public const string AutoCooking = "AutoCooking";
        public const string PointerSpeed = "PointerSpeed";
        public const string IsAutoCookingUnlocked = "IsAutoCookingUnlocked";
        public const string LanguageKey = "LanguageSettings";
        public const string VideoWatched = "VideoWatched";
    }
}