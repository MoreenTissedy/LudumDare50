using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public enum WardrobeCellState
    {
        None,
        Owned,
        NewlyUnlocked,
        AvailableToBuy,
        UnavailableToBuy,
        Locked
    }
    
    public class WardrobeCell : GrowOnMouseEnter
    {
        [SerializeField] private Image skinPreview;
        [SerializeField] private Image background;
        [SerializeField] private Image Highlight;
        [SerializeField] private Image SmallHighlight;

        public Material lockedMaterial;
        
        [SerializeField] private Sprite availableSkinBackground;
        [SerializeField] private Sprite lockedSkinBackground;

        private WardrobeCellState currentState;
        
        private SkinSO skin;
        public SkinSO Skin => skin;
        
        [Inject] private SoundManager sound;

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
            SmallHighlight.enabled = false;
            Highlight.enabled = false;
            background.sprite = lockedSkinBackground;

            switch (currentState)
            {
                case WardrobeCellState.UnavailableToBuy:
                case WardrobeCellState.None:
                    break;
                case WardrobeCellState.AvailableToBuy:
                    SmallHighlight.enabled = true;
                    break;
                case WardrobeCellState.Owned:
                    background.sprite = availableSkinBackground;
                    break;
                case WardrobeCellState.NewlyUnlocked:
                    background.sprite = availableSkinBackground;
                    //TODO
                    break;
                case WardrobeCellState.Locked:
                    skinPreview.material = lockedMaterial;
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
            if (currentState == WardrobeCellState.Locked)
            {
                return;
            }
            
            base.OnPointerClick(eventData);
            sound.Play(Sounds.MenuClick);
            OnClick?.Invoke(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (currentState == WardrobeCellState.Locked)
            {
                return;
            }
            base.OnPointerEnter(eventData);
        }
    }
}