namespace Save
{
    public class PlayerProgress
    {
        public string[] UnlockedRecipes;
        public string[] UnlockedEndings;
        public int CurrentRound;
    }

    public class PlayerStoryProgress
    {
        public string[] Milestones;
        public bool CovenIntroShown;
        public bool IsAutoCookingUnlocked;
    }
}