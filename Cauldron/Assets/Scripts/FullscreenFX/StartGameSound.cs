using JetBrains.Annotations;
using UnityEngine;

namespace CauldronCodebase
{
    public class StartGameSound : MonoBehaviour
    {
        [SerializeField] private SoundManager sound;

        [UsedImplicitly]
        public void PlaySpark()
        {
            sound.Play(Sounds.StartSpark);
        }

        [UsedImplicitly]
        public void PlayFlame()
        {
            sound.Play(Sounds.StartFlash);
        }
    }
}