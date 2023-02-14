using System;
using System.Linq;

namespace CauldronCodebase
{
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