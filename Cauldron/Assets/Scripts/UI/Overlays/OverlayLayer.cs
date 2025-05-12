using UnityEngine;

namespace CauldronCodebase
{
    public class OverlayLayer: MonoBehaviour
    {
        private bool locked;
        private IOverlayElement[] items;

        private void OnValidate()
        {
            var componentsInChildren = GetComponentsInChildren<OverlayLayer>();
            if (componentsInChildren.Length > 1)
            {
                Debug.LogError($"[NESTED OVERLAY LAYERS] {gameObject.name} has {componentsInChildren.Length - 1} nested layer(s), first one is {componentsInChildren[1].gameObject.name}");
            }
        }

        private void Awake()
        {
            items = GetComponentsInChildren<IOverlayElement>();
        }

        public void Lock(bool on)
        {
            locked = on;
            foreach (var element in items)
            {
                element.Lock(on);
            }
        }
    }
}