using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New_ending", menuName = "Ending", order = 9)]
    public class Ending : ScriptableObject
    {
        public string text;
        public Sprite image;
    }
}