using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal;

namespace CauldronCodebase
{
    public enum WardrobeCellState
    {
        Owned,
        Available,
        Unavailable,
        NewlyUnlocked,
    }
    
    public class WardrobeCell : GrowOnMouseEnter
    {
        [SerializeField] private Image skinPreview;
        [SerializeField] private Image background;
        [SerializeField] private Image Highlight;

        [SerializeField] private Material lockedSkinMaterial;
        
        [SerializeField] private Sprite availableSkinBackground;
        [SerializeField] private Sprite lockedSkinBackground;

        private WardrobeCellState currentState;
        
        private SkinSO skin;
        public SkinSO Skin => skin;

        public event Action<WardrobeCell> OnClick; 

        public void Setup(SkinSO newSkin, WardrobeCellState wardrobeCellState)
        {
            skin = newSkin;
            skinPreview.sprite = skin.PreviewIcon;
            
            SetState(wardrobeCellState);
        }

        public void SetState(WardrobeCellState newState)
        {
            currentState = newState;

            switch (currentState)
            {
                case WardrobeCellState.Unavailable:
                    skinPreview.material = lockedSkinMaterial;
                    background.sprite = lockedSkinBackground;
                    break;
                case WardrobeCellState.Available:
                    skinPreview.material = null;
                    background.sprite = availableSkinBackground;
                    break;
                case WardrobeCellState.Owned:
                    //TODO: Добавить индикацию для имеющихся у игрока скинов
                    skinPreview.material = null;
                    background.sprite = availableSkinBackground;
                    break;
                case WardrobeCellState.NewlyUnlocked:
                    //TODO: Добавить индикацию для только что открытых скинов
                    skinPreview.material = null;
                    background.sprite = availableSkinBackground;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ToggleSelect(bool state)
        {
            Highlight.enabled = state;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if(currentState == WardrobeCellState.Unavailable) return;
            
            base.OnPointerClick(eventData);
            
            OnClick?.Invoke(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if(currentState == WardrobeCellState.Unavailable) return;
            base.OnPointerEnter(eventData);
        }
    }
}