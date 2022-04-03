using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New_Encounter", menuName = "Encounter", order = 1)]
    public class Encounter : ScriptableObject
    {
        public Villager[] villager; 
        public string text;
        public Potions requiredPotion;
        [Header("Right potion brewed")] public int moneyBonus; 
        public int fearBonus, fameBonus;
        public Encounter[] bonusCard;
        [Header("Wrong potion brewed")] public int moneyPenalty;
        public int fearPenalty, famePenalty;
        public Encounter[] penaltyCard;
        [Header("Second potion variant")] 
        public bool useSecondVariant;
        public Potions requiredPotion2;
        public int moneyBonus2; 
        public int fearBonus2, fameBonus2;
        public Encounter[] bonusCard2;

        [HideInInspector] public Villager actualVillager;

        public void Init()
        {
            if (villager.Length > 0)
            {
                int random = Random.Range(0, villager.Length);
                actualVillager = villager[random];
            }
        }

        public static Encounter GetRandom(Encounter[] set)
        {
            if (set.Length > 0)
            {
                int random = Random.Range(0, set.Length);
                return (set[random]);
            }

            return null;
        }
    }
}