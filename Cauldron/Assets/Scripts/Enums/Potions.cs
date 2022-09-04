using System;
using System.Linq;

namespace CauldronCodebase
{
    public enum Potions
    {   // magic
        Love, Unlove,
        Strength, Curse,
        Poison, Healing,
        Chameleon, Beauty,
        Sleep, Wake,
        Laughter, VoiceChange, Memory,
        Flying, Foolishness,
        Numb, Perception, Rejuvenation,
        Speed, Clumsiness,
        //new magic recipes go here

        // not magic
        MushroomSoup = 51, HotSpice = 52, JellyInTheTail = 53,
        MeatSoup = 54, NettleShchi = 55, NutritiousSoup = 56, RatPottage = 57,
        PaleLiquor = 58, LeafTea = 59, Kombucha = 60, RootTincture = 61, ScreamingTea = 62,
        SurpriseMushrooms = 63, TripleDecoction = 64, ViralChowder = 65,
        CaleidoscopicBooze = 66, HideousLiquid = 67, HotStuff = 68,
        RatOnRoots = 69, SapOfEnlightenment = 70, SporadicGoo = 71,
        
        //special filters
        Placebo = 99, DEFAULT = 100, FOOD = 101, DRINK = 102, ALCOHOL = 103, MAGIC = 104, NONMAGIC = 105  
    }

    public static class PotionFilter
    {
        public static Potions[] Get(Potions filter)
        {
            if (filter == Potions.FOOD)
            {
                return new[]
                {
                    Potions.MushroomSoup,
                    Potions.HotSpice,
                    Potions.JellyInTheTail,
                    Potions.MeatSoup,
                    Potions.NettleShchi,
                    Potions.NutritiousSoup,
                    Potions.RatPottage,
                    Potions.RatOnRoots,
                    Potions.SurpriseMushrooms,
                    Potions.ViralChowder,
                };
            }

            if (filter == Potions.DRINK)
            {
                return new[]
                {
                    Potions.PaleLiquor,
                    Potions.LeafTea,
                    Potions.Kombucha,
                    Potions.RootTincture,
                    Potions.ScreamingTea,
                    Potions.TripleDecoction,
                    Potions.CaleidoscopicBooze,
                    Potions.HideousLiquid,
                    Potions.HotStuff,
                    Potions.SapOfEnlightenment,
                    Potions.SporadicGoo
                };
            }
            
            if (filter == Potions.ALCOHOL)
            {
                return new[]
                {
                    Potions.PaleLiquor,
                    Potions.RootTincture,
                    Potions.TripleDecoction,
                    Potions.CaleidoscopicBooze,
                    Potions.HotStuff,
                    Potions.SporadicGoo
                };
            }

            if (filter == Potions.MAGIC)
            {
                var potionsArray = Enum.GetValues(typeof(Potions)) as Potions[];
                return potionsArray.Where(x => (int) x < 50).ToArray();
            }
            
            if (filter == Potions.NONMAGIC)
            {
                var potionsArray = Enum.GetValues(typeof(Potions)) as Potions[];
                return potionsArray.Where(x => (int) x >= 50 && (int) x < 99).ToArray();
            }
            
            return null;
        } 
    }
}