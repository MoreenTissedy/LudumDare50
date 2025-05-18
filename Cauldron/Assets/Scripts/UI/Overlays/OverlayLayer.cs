using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace CauldronCodebase
{
    public class OverlayLayer: MonoBehaviour
    {
        public bool locked;
        [ReadOnly] public int childCount;
        
        public List<IOverlayElement> items;

        private void OnValidate()
        {
            var componentsInChildren = GetComponentsInChildren<OverlayLayer>(true);
            if (componentsInChildren.Length > 1)
            {
                Debug.LogError($"[NESTED OVERLAY LAYERS] {gameObject.name} has {componentsInChildren.Length - 1} nested layer(s), first one is {componentsInChildren[1].gameObject.name}");
            }
        }

        private void Awake()
        {
            items = GetComponentsInChildren<IOverlayElement>(true).ToList();
            childCount = items.Count;
            if (locked)
            {
                Lock(true);
            }
        }

        public void Lock(bool on)
        {
            locked = on;
            foreach (var element in items)
            {
                element.Lock(on);
            }
        }

        public void Register(IOverlayElement element)
        {
            items.Add(element);
        }
    }
}