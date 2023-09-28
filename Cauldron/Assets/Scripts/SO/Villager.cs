using UnityEngine;

namespace CauldronCodebase
{
    [CreateAssetMenu(fileName = "New_Villager", menuName = "Villager", order = 0)]
    public class Villager : ScriptableObjectInDictionary
    {
        public Visitor visitorPrefab;
        public int patience = 3;
        public VisitorSounds sounds;
    }
}