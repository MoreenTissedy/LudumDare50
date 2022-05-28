using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class PotionSelect : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RecipeBookEntry entry;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!entry)
            {
                entry = GetComponentInParent<RecipeBookEntry>();
            }
        }
        #endif

        public void OnPointerClick(PointerEventData eventData)
        {
            RecipeBook.instance.SwitchHighlight(entry);
        }
    }
}