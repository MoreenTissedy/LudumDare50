using System;
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
    
    public enum BookSound
    {
        Open,
        Close,
        Left,
        Right
    }

    [Serializable]
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

        public void PlayBook(BookSounds collection, BookSound type)
        {
            AudioClip sound = null;
            switch (type)
            {
                case BookSound.Open:
                    sound = collection.Open;
                    break;
                case BookSound.Close:
                    sound = collection.Close;
                    break;
                case BookSound.Left:
                    sound = collection.Left;
                    break;
                case BookSound.Right:
                    sound = collection.Right;
                    break;
            } 
            Debug.LogError(sound?.name ?? "not defined");
        }
    }
}