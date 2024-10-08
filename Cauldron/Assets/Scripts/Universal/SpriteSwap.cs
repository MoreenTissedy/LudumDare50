using UnityEngine;
using UnityEngine.UI;

namespace Universal
{
    public class SpriteSwap: MonoBehaviour
    {
        public Image image;
        public Sprite sprite;

        public void Swap()
        {
            image.sprite = sprite;
        }
    }
}