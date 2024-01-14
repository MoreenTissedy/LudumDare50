using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EasyLoc
{
    [RequireComponent(typeof(Image))]
    public class ImageLoc: MonoBehaviour
    {
        [SerializeField] private Sprite[] sprites;
        [Localize] [SerializeField] private string imageName;

        private string lastName;

        void Update()
        {
            if (imageName != lastName)
            {
                lastName = imageName;
                Sprite first = null;
                foreach (var x in sprites)
                {
                    if (x.name.Equals(imageName.Trim()))
                    {
                        first = x;
                        break;
                    }
                }

                GetComponent<Image>().sprite = first;
            }
        }
    }
}