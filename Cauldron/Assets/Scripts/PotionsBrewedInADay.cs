using System;
using System.Collections.Generic;

namespace CauldronCodebase
{
    [Serializable]
    public class PotionsBrewedInADay
    {
        public List<Potions> PotionsList = new List<Potions>(15);
        public int WrongPotions = 0;
    }
}