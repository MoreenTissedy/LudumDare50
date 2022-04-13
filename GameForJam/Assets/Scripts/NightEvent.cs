using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New night event", menuName = "Night event", order = 8)]
    public class NightEvent : ScriptableObject
    {
        [TextArea(8,8)]
        public string flavourText;
        public int moneyModifier, fearModifier, fameModifier;
    }
}