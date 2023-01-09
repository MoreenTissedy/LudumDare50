using UnityEngine;

namespace CauldronCodebase.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public AudioClip success;
        public AudioClip failure;
        public static SoundManager theOne;

        private void Awake()
        {
            theOne = this;
        }

        public void PlaySuccess()
        {
            GetComponent<AudioSource>().PlayOneShot(success);
        }

        public void PLayFailure()
        {
            GetComponent<AudioSource>().PlayOneShot(failure);
        }
    }
}