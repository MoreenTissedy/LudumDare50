using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Universal;

namespace CauldronCodebase
{
    public interface IAchievementManager
    {
        bool TryUnlock(string id);
        bool TryUnlock(NightEvent nightEvent);
        void ClearAll();
    }

    public static class AchievIdents
    {
        public const string FIRST_POTION = "first potion";
        public const string FIRST_UNLOCK = "first unlock";
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
            {"bastard-success", "chalice story"},
            {"king-reject", "neutrality"},
            {"bishop-reject", "neutrality"},
            {"ixbt-final", "rainbow drakkar"},
            {"ixbt-guard-success", "vampire slayer"},
            {"tavern.wake", "bard"},
            {"TaxCollectorFoundDead", "death and taxes"},
            {"CowPerformReturned", "cow"},
            {"scarecrow-knight-drunk", "scarecrows"},
            {"old_knight_event.2","knight"},
            {"moonshine-stealing-fairies-caught","pixies"},
            {"Doctor_4_Rejuvenation","doctor"},
            {"mary.91","Mary"},
        };
    }

    public class AchievementManager : IAchievementManager
    {
        public bool TryUnlock(string id)
        {
            if (!SteamConnector.LoggedIn || !SteamClient.IsLoggedOn)
            {
                return false;
            }
            
            var achievement = SteamUserStats.Achievements.FirstOrDefault(x => x.Identifier == id);
            if (string.IsNullOrWhiteSpace(achievement.Name))
            {
                Debug.LogError($"Achievement {id} not found!");
                return false;
            } 
            if (achievement.State == true)
            {
                return false;
            }
            bool unlocked = achievement.Trigger();
            if (unlocked)
            {
                Debug.Log($"Achievement {id} unlocked!");
                SteamUserStats.StoreStats();
            }
            else Debug.LogError($"Achievement {id} failed to unlock!");
            return unlocked;
        }

        public bool TryUnlock(NightEvent nightEvent)
        {
            if (!AchievIdents.EVENT_NAMES_TO_ACHIEVEMENTS.TryGetValue(nightEvent.name, out string tag))
            {
                return false;
            }
            return TryUnlock(tag);
        }

        public void ClearAll()
        {
            foreach (Achievement achievement in SteamUserStats.Achievements)
            {
                achievement.Clear();
            }
            SteamUserStats.StoreStats();
            Debug.LogError("Achievements cleared!");
        }
    }
}