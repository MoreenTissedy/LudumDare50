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
}