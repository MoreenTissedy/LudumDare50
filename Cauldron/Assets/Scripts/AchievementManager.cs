using UnityEngine;

namespace CauldronCodebase
{
    public interface IAchievementManager
    {
        bool TryUnlock(string id);
    }

    public static class AchievIdents
    {
        public const string MAGIC_ALL = "magic recipes";
        public const string FOOD_ALL = "food recipes";
        public const string EXPERIMENTS_ALL = "experiments";
    }

    public class AchievementManager : IAchievementManager
    {
        public bool TryUnlock(string id)
        {
            Debug.LogError("try unlock "+id);
            return true;
        }
    }
}