namespace CauldronCodebase
{
    public static class StoryTagHelper
    {
        public static bool Check(Encounter card, GameDataHandler gameDataHandler)
        {
            return Check(card.requiredStoryTag, gameDataHandler);
        }
        
        public static bool Check(string tagLine, GameDataHandler gameDataHandler)
        {
            if (tagLine is null || string.IsNullOrWhiteSpace(tagLine.Trim()))
            {
                return true;
            }

            string[] tags = tagLine.Split(',');
            bool valid = true;
            foreach (var tag in tags)
            {
                string trim = tag.Trim();
                if (trim == EndingsProvider.LOW_FAME || trim == EndingsProvider.LOW_FEAR ||
                    trim == EndingsProvider.HIGH_FAME || trim == EndingsProvider.HIGH_FEAR)
                {
                    continue;
                }
                if (trim.StartsWith("!"))
                {
                    valid = valid && !gameDataHandler.storyTags.Contains(trim.TrimStart('!'));
                }
                else
                {
                    valid = valid && gameDataHandler.storyTags.Contains(trim);
                }
            }
            return valid;
        }

        public static bool CovenSavingsEnabled(GameDataHandler gameDataHandler)
        {
            return gameDataHandler.storyTags.Contains("circle money") && !CovenFeatureUnlocked(gameDataHandler);
        }
        
        public static bool CovenQuestEnabled(GameDataHandler gameDataHandler)
        {
            return gameDataHandler.storyTags.Contains("circle quest") && !CovenSavingsEnabled(gameDataHandler) && !CovenFeatureUnlocked(gameDataHandler);
        }

        public static bool CovenFeatureUnlocked(GameDataHandler gameDataHandler)
        {
            return gameDataHandler.storyTags.Contains("circle");
        }
    }
}