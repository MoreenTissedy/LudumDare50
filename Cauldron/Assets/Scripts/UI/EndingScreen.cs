using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class EndingScreen : MonoBehaviour
    {
        public Text text;
        public Image image;
        
        private void Start()
        {
            Hide();
        }

        public void Show(Ending ending)
        {
            text.text = ending.text;
            image.sprite = ending.image;
            gameObject.SetActive(true);
        }
        

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}