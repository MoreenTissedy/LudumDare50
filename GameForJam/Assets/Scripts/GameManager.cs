using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public IngredientsData ingredientsBook;

        private void Awake()
        {
            if (instance is null)
                instance = this;
            else
            {
                Debug.LogError("double singleton:"+this.GetType().Name);
            }
        }
        
        public Status money, fear, fame;
        public int defaultMoneyBonus = 5;
        public int defaultFameBonus = 5;
    }
}