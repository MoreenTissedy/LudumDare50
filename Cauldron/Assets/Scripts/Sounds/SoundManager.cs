using UnityEngine;

namespace CauldronCodebase
{
    public enum Sounds
    {
        Music,
        Bubbling,
        Splash,
        PotionReady,
        Success,
        Failure
    }

    public struct BookSounds
    {
        public AudioClip Open;
        public AudioClip Close;
        public AudioClip Left;
        public AudioClip Right;
    }

    [CreateAssetMenu]
    public class SoundManager : ScriptableObject
    {
        public AudioClip[] sounds;

        public void Init()
        {
            Debug.LogError("start music");
        }

        public void Play(Sounds sound)
        {
            Debug.LogError(sounds[(int) sound].name);
        }
    }
}