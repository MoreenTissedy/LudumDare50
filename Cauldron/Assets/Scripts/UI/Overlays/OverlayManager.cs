using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace CauldronCodebase
{
    public enum Layers
    {
        Unknown,
        Base,
        RecipeBook,
        NightScreen,
        EndingScreen,
        Wardrobe,
        SkinShop,
        Tutorial
    }
    
    public class OverlayManager: MonoBehaviour
    {
        [SerializeField] private OverlayLayer baseLayer;

        private Stack<(OverlayLayer, Layers)> layers = new Stack<(OverlayLayer, Layers)>();

        public Layers GetCurrentLayer => layers.Peek().Item2;

        [ReadOnly]
        public Layers[] debugStack;

        [Button("Debug stack")]
        public void SeeStack()
        {
            debugStack = layers.Select(x => x.Item2).ToArray();
        }

        private void Awake()
        {
            layers.Push((baseLayer, Layers.Base));
        }

        public void AddLayer(OverlayLayer layer, Layers layerId = 0)
        {
            var topLayer = layers.Peek();
            if (topLayer.Item1 == layer)
            {
                Debug.LogError("[Overlay Layers] Trying to add the layer that is already on top of the stack");
                return;
            }

            if (topLayer.Item2 == Layers.Tutorial)
            {
                layers.Pop();
                layers.Push((layer, layerId));
                layers.Push(topLayer);
                layer.Lock(true);
                Debug.Log("[Overlay Layers] Added new layer under the tutorial layer");
                return;
            }
            
            topLayer.Item1.Lock(true);
            layers.Push((layer, layerId));
            layers.Peek().Item1.Lock(false);
        }

        public void RemoveLayer(OverlayLayer layer)
        {
            if (layers.Peek().Item1 != layer)
            {
                Debug.LogError("[Overlay Layers] Trying to remove layer that is NOT on top of the stack "+layer.gameObject.name);
                return;
            }
            layer.Lock(true);
            layers.Pop();
            layers.Peek().Item1.Lock(false);
        }
        
        public void LockCurrentLayer(bool value)
        {
            layers.Peek().Item1.Lock(value);
        }
    }
}