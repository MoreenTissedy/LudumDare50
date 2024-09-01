using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal;

namespace CauldronCodebase
{
    public class WardrobeCell : GrowOnMouseEnter
    {
        [SerializeField] private Image skinPreview;
        [SerializeField] private Image background;
        [SerializeField] private Image Highlight;

        [SerializeField] private Material lockedSkinMaterial;
        
        [SerializeField] private Sprite availableSkinBackground;
        [SerializeField] private Sprite lockedSkinBackground;

        private bool isAvailable = true;
        
        private SkinSO skin;
        public SkinSO Skin => skin;

        public event Action<WardrobeCell> OnClick; 

        public void Setup(SkinSO newSkin, bool unlock)
        {
            skin = newSkin;
            skinPreview.sprite = skin.PreviewIcon;
            
            if (unlock)
            {
                isAvailable = true;
                skinPreview.material = null;
                background.sprite = availableSkinBackground;
            }
            else
            {
                isAvailable = false;
                skinPreview.material = lockedSkinMaterial;
                background.sprite = lockedSkinBackground;
            }
        }

        public void ToggleSelect(bool state)
        {
            Highlight.enabled = state;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if(!isAvailable) return;
            
            base.OnPointerClick(eventData);
            
            OnClick?.Invoke(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!isAvailable) return;
            base.OnPointerEnter(eventData);
        }
    }
}