using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New_Villager", menuName = "Villager", order = 0)]
    public class Villager : ScriptableObject
    {
        public Sprite image;
    }
}