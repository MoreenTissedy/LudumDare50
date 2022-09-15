using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class AttemptEntry: MonoBehaviour
    {
        [Inject]
        private IngredientsData data;
        
        [SerializeField] private Image image1, image2, image3;

        private void Awake()
        {
            Clear();
        }

        public void Display(Ingredients[] attempt)
        {
            if (attempt.Length != 3)
            {
                Debug.LogError("failed attempts are recorded as an array of 3 ingredients");
                return;
            }
            
            image1.sprite = data.Get(attempt[0]).image;
            image2.sprite = data.Get(attempt[1]).image;
            image3.sprite = data.Get(attempt[2]).image;
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
        }
    }
}