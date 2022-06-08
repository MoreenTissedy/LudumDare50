using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class CauldronDropZone : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField]
        private Cauldron pot;

        private void OnValidate()
        {
            if (pot is null)
            {
                pot = GetComponentInParent<Cauldron>();
            } 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pot?.PointerEntered();
        }
    }
}