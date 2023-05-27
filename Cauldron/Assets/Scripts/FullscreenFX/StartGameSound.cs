using UnityEngine;

namespace CauldronCodebase
{
    public class StartGameSound : MonoBehaviour
    {
        [SerializeField] private SoundManager sound;

        public void PlaySpark()
        {
            sound.Play(Sounds.StartSpark);
        }

        public void PlayFlame()
        {
            sound.Play(Sounds.StartFlash);
        }
    }
}