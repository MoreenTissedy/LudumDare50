using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New_Villager", menuName = "Villager", order = 0)]
    public class Villager : ScriptableObjectWithId
    {
        public Sprite image;
        public int patience = 3;
        public VisitorSounds sounds;
    }
}