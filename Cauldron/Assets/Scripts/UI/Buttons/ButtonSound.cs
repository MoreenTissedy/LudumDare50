using CauldronCodebase;
using Zenject;

namespace Universal
{
    public class ButtonSound : IAnimatedButtonComponent
    {
        [Inject] private SoundManager sound;

        public override void Select()
        {
            sound.Play(Sounds.MenuFocus);
        }

        public override void Activate()
        {
            sound.Play(Sounds.MenuClick);
        }

        public override void ChangeInteractive(bool isInteractive){}
        public override void Unselect(){}
    }
}