using System;
using FMODUnity;
using UnityEngine;

namespace CauldronCodebase
{
    public enum Sounds
    {
        Music,
        Bubbling,
        Splash,
        PotionReady,
        PotionUnlock,
        PotionPopupClick,
        PotionSuccess,
        PotionFailure,
        BookFocus,
        BookClick,
        BookSwitch,
        MenuFocus,
        MenuClick
    }

    public enum VisitorSound
    {
        Door,
        Enter,
        Exit,
        Speech
    }

    public enum CatSounds
    {
        PurrShort,
        PurrLong,
        Conversation,
        Attention,
        Annoyed
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
        public EventReference Open;
        public EventReference Close;
        public EventReference Left;
        public EventReference Right;
    }
    
    [Serializable]
    public struct VisitorSounds
    {
        public EventReference Door;
        public EventReference Enter;
        public EventReference Exit;
        public EventReference Speech;
    }

    [CreateAssetMenu]
    public class SoundManager : ScriptableObject
    {
        public EventReference[] sounds;
        public VisitorSounds defaultVisitorSounds;

        public void Init()
        {
            Play(Sounds.Music);
            Play(Sounds.Bubbling);
            RuntimeManager.GetVCA("vca:/Music").setVolume(0.3f);
        }

        public void Play(Sounds sound)
        {
            EventReference reference = sounds[(int) sound];
            if (reference.IsNull)
            {
                return;
            }
            RuntimeManager.PlayOneShot(reference);
        }

        public void PlayBook(BookSounds collection, BookSound type)
        {
            EventReference sound = new EventReference();
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
            RuntimeManager.PlayOneShot(sound);
        }
        
        public void PlayVisitor(VisitorSounds collection, VisitorSound type)
        {
            EventReference sound = new EventReference();
            switch (type)
            {
                case VisitorSound.Enter:
                    sound = collection.Enter.IsNull ? defaultVisitorSounds.Enter : collection.Enter;
                    break;
                case VisitorSound.Door:
                    sound = collection.Door.IsNull ? defaultVisitorSounds.Door : collection.Door;
                    break;
                case VisitorSound.Exit:
                    sound = collection.Exit.IsNull ? defaultVisitorSounds.Exit : collection.Exit;
                    break;
                case VisitorSound.Speech:
                    sound = collection.Speech.IsNull ? defaultVisitorSounds.Speech : collection.Speech;
                    break;
            } 
            RuntimeManager.PlayOneShot(sound);
        }
    }
}