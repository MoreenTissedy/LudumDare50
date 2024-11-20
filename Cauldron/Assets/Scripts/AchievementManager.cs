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
        bool SetStat(string id, int stat);
    }

    public struct PotionsUsedAchiv
    {
        public Potions potion;
        public int count;
        public string key;

        public PotionsUsedAchiv(Potions potion, int count, string key)
        {
            this.potion = potion;
            this.count = count;
            this.key = key;
        }
    }

    public static class AchievIdents
    {
        public const string FIRST_POTION = "first potion";
        public const string FIRST_UNLOCK = "first unlock";
        public const string MAGIC_ALL = "magic recipes";
        public const string FOOD_ALL = "food recipes";
        public const string EXPERIMENTS_ALL = "experiments";
        public const string VISITORS_ALL = "all visitors";
        public const string SILVER_DAYS = "silver days";
        public const string GOLD_DAYS = "gold days";

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
            {"Lamps-numbness", "lamps"},
            {"Succub-default", "succubus"},
            {"Recipe_for_revenge-surprise_mushrooms", "culinary"}
        };

        public static readonly List<PotionsUsedAchiv> USED_POTION = new List<PotionsUsedAchiv>()
        {
            new PotionsUsedAchiv(Potions.Poison, 10, "potion poison 1"),
            new PotionsUsedAchiv(Potions.Poison, 25, "potion poison 2"),
            new PotionsUsedAchiv(Potions.Love, 10, "potion love 1"),
            new PotionsUsedAchiv(Potions.Love, 25, "potion love 2"),
            new PotionsUsedAchiv(Potions.Memory, 10, "potion memory 1"),
            new PotionsUsedAchiv(Potions.Memory, 25, "potion memory 2"),
            new PotionsUsedAchiv(Potions.Flying, 10, "potion flight 1"),
            new PotionsUsedAchiv(Potions.Flying, 25, "potion flight 2"),
            new PotionsUsedAchiv(Potions.Perception, 10, "potion perception 1"),
            new PotionsUsedAchiv(Potions.VoiceChange, 10, "potion voice 1"),
            new PotionsUsedAchiv(Potions.Laughter, 10, "potion laughter 1"),
        };
    }

    public class AchievementManager : IAchievementManager
    {
        public bool TryUnlock(string id)
        {
            //if (!SteamConnector.LoggedIn || !SteamClient.IsLoggedOn)
            {
                //return false;
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

        public bool SetStat(string id, int stat)
        {
            if (!SteamConnector.LoggedIn || !SteamClient.IsLoggedOn)
            {
                return false;
            }
            SteamUserStats.SetStat(id, stat);
            SteamUserStats.StoreStats();
            return true;
        }
    }
}