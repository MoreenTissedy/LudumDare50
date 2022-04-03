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
        public Encounter bonusCard;
        [Header("Wrong potion brewed")] public int moneyPenalty;
        public int fearPenalty, famePenalty;
        public Encounter penaltyCard;

        [HideInInspector] public Villager actualVillager;

        public void Init()
        {
            if (villager.Length > 0)
            {
                int random = Random.Range(0, villager.Length);
                actualVillager = villager[random];
            }
        }
    }
}