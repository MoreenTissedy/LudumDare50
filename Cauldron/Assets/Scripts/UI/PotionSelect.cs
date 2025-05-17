using UnityEngine;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class PotionSelect : AnimatedButtonComponent
    {
        [SerializeField] private FlexibleButton button;
        [SerializeField] private bool clicable;

        [Inject] private RecipeBook book;
        [Inject] private SoundManager soundManager;

        private void OnEnable()
        {
            button.IsInteractive = !book.isNightBook && clicable;
        }

        public override void Select()
        {
            soundManager.Play(Sounds.BookFocus);
        }

        public override void Activate()
        {
            book.SwitchHighlight(GetComponentInParent<RecipeBookEntry>());
        }

        public override void Unselect(){}        
        public override void ChangeInteractive(bool isInteractive){}
    }
}