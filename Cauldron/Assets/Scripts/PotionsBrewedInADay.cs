using System;
using System.Collections.Generic;

namespace CauldronCodebase
{
    [Serializable]
    public class PotionsBrewedInADay
    {
        public List<string> PotionsList = new List<string>(15);
        public int WrongPotions = 0;
    }
}