using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
namespace CauldronCodebase
{
    public class WardrobeButton : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private Wardrobe wardrobe;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            wardrobe.OpenBook();
        }
    }
}
