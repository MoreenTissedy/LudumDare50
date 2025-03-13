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
    
    public class WardrobeCell : MonoBehaviour
    {
        [SerializeField] private Image skinPreview;
        [SerializeField] private Image background;
        [SerializeField] private Image Highlight;
        [SerializeField] private Image SmallHighlight;

        public Material lockedMaterial;
        
        [SerializeField] private Sprite availableSkinBackground;
        [SerializeField] private Sprite lockedSkinBackground;

        [Space] [SerializeField] private FlexibleButton button;

        private WardrobeCellState currentState;
        
        private SkinSO skin;
        public SkinSO Skin => skin;
        
        [Inject] private SoundManager sound;

        public event Action<WardrobeCell> OnClick;

        private void Awake()
        {
            button.OnClick += Click;
        }

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
            button.IsInteractive = true;

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
                    button.IsInteractive = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ToggleSelect(bool state)
        {
            Highlight.enabled = state;
        }

        public void Click()
        {
            sound.Play(Sounds.MenuClick);
            OnClick?.Invoke(this);
        }
    }
}