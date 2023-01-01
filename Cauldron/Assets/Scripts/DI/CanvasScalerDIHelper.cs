using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasScalerDIHelper : MonoBehaviour
    {
        public string sortingLayerName = "UI";
        
        [Inject]
        private void SetCamera(Camera main)
        {
            var component = GetComponent<Canvas>();
            component.worldCamera = main;
            component.sortingLayerName = sortingLayerName;
        }
    }
}