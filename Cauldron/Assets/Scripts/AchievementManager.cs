using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
    public interface IAchievementManager
    {
        bool TryUnlock(string id);
        bool TryUnlock(NightEvent nightEvent);
    }

    public static class AchievIdents
    {
        public const string MAGIC_ALL = "magic recipes";
        public const string FOOD_ALL = "food recipes";
        public const string EXPERIMENTS_ALL = "experiments";
        public const string VISITORS_ALL = "all visitors";
        public const string SPEND_COINS = "in circulation";
        public const int SPEND_TARGET = 200;

        public static readonly Dictionary<string, string> EVENT_NAMES_TO_ACHIEVEMENTS = new Dictionary<string, string>()
        {
            {"pig-WhenPigsFly", "when pigs fly"},
            {"WomenLostMemory", "love affairs"},
            {"ManDiedHeartAttack", "dead man"},
            {"Cultist_3", "cultists"},
            {"stolen_rooster_event.3", "prince rooster"},
            {"mushroom-story-event.2", "mushroom circle"},
            {"Miner-clumsy", "underground river"},
            {"roses-forget", "total oblivion"},
            {"jack-feast-success", "trick or treat"},
            {"Painter_3_Love", "art explosion"},
            {"FoolDied", "fool death"},
            {"bastard-2-success", "chalice story"},
            {"king-reject", "neutrality"},
            {"bishop-reject", "neutrality"},
            {"ixbt final", "rainbow drakkar"},
            {"ixbt-guard-success", "vampire slayer"},
            {"tavern.wake", "bard"}
        };
    }

    public class AchievementManager : IAchievementManager
    {
        public bool TryUnlock(string id)
        {
            Debug.LogError("try unlock "+id);
            return true;
        }

        public bool TryUnlock(NightEvent nightEvent)
        {
            if (!AchievIdents.EVENT_NAMES_TO_ACHIEVEMENTS.TryGetValue(nightEvent.name, out string tag))
            {
                return false;
            }
            Debug.LogError("try unlock "+tag);
            return true;
        }
    }
}