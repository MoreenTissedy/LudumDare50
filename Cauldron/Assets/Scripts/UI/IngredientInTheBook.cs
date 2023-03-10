using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CauldronCodebase
{
    public class IngredientInTheBook : MonoBehaviour
    {
        public Image topImage;
        public Image bottomImage;
        public TextMeshProUGUI text;

        [SerializeField] private RectTransform layoutGroup;

        [ContextMenu("Rebuild")]
        public void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
        }
    }
}