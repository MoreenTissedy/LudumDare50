using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New condition", menuName = "Total potion check condition", order = 8)]
    public class NightCondition : ScriptableObject
    {
        [Header("Conditions work only once")]
        public Potions type;
        public int threshold = 3;
        public string flavourText;
        public int moneyModifier, fearModifier, fameModifier;
        public Encounter bonusCard;
    }
}