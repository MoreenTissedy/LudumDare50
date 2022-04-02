using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New_Encounter", menuName = "Encounter", order = 1)]
    public class Encounter : ScriptableObject
    {
        public Villager villager; 
        public string text;
        public Potions requiredPotion;
        [Header("Right potion brewed")]
        public int moneyBonus, fearBonus, fameBonus;
        public Encounter[] bonusCards;
        [Header("Wrong potion brewed")] 
        public int moneyPenalty, fearPenalty, famePenalty;
        public Encounter[] penaltyCards;
    }
}