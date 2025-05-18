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
        SkinShop
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
            if (layers.Peek().Item1 == layer)
            {
                Debug.LogError("[Overlay Layers] Trying to add the layer that is already on top of the stack");
                return;
            }
            layers.Peek().Item1.Lock(true);
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
    }
}