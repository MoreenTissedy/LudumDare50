using System.Collections.Generic;
using UnityEngine;

namespace CauldronCodebase
{
    public class MenuOverlayManager: MonoBehaviour
    {
        [SerializeField] private OverlayLayer baseMenuLayer;
        [SerializeField] private AuthorsMenu authorsMenu;
        [SerializeField] private SettingsMenu settingsMenu;

        private Stack<OverlayLayer> layers = new Stack<OverlayLayer>();

        private void Awake()
        {
            layers.Push(baseMenuLayer);
        }

        public void OpenSettings()
        {
            settingsMenu.Open();
            AddLayer(settingsMenu.mainLayer);
        }

        public void OpenAuthors()
        {
            authorsMenu.Open();
            AddLayer(authorsMenu.GetComponent<OverlayLayer>());
        }

        public void AddLayer(OverlayLayer layer)
        {
            if (layers.Peek() == layer)
            {
                Debug.LogError("[Overlay Layers] Trying to add the layer that is already on top of the stack");
                return;
            }
            layers.Peek().Lock(true);
            layers.Push(layer);
            layers.Peek().Lock(false);
        }

        public void RemoveLayer(OverlayLayer layer)
        {
            if (layers.Peek() != layer)
            {
                Debug.LogError("[Overlay Layers] Trying to remove layer that is NOT on top of the stack");
                return;
            }
            layers.Pop();
            layers.Peek().Lock(false);
        }
    }
}