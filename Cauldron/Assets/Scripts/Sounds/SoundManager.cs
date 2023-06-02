using System;
using FMODUnity;
using UnityEngine;

namespace CauldronCodebase
{
    //Please add new sounds to the end of the list
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
        MenuClick,
        TimerBreak,
        StartFlash,
        GameEnd,
        NightCardEnter,
        NightCardExit,
        StartSpark,
        StartDay,
        EndDay,
    }

    public enum VisitorSound
    {
        Door,
        Enter,
        Exit,
        Speech
    }

    public enum CatSound
    {
        Purr,
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
    
    [Serializable]
    public struct CatSounds
    {
        public EventReference PurrShort;
        public EventReference PurrLong;
        public EventReference Conversation;
        public EventReference Attention;
        public EventReference Annoyed;
    }

    [CreateAssetMenu]
    public class SoundManager : ScriptableObject
    {
        public EventReference[] sounds;
        public VisitorSounds defaultVisitorSounds;
        public CatSounds catSounds;

        public void Start()
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

        public void PlayCat(CatSound type, bool prolonged = false)
        {
            EventReference sound = new EventReference();
            switch (type)
            {
                case CatSound.Purr:
                    sound = prolonged ? catSounds.PurrLong : catSounds.PurrShort;
                    break;
                case CatSound.Conversation:
                    sound = catSounds.Conversation;
                    break;
                case CatSound.Attention:
                    sound = catSounds.Attention;
                    break;
                case CatSound.Annoyed:
                    sound = catSounds.Annoyed;
                    break;
            }
            RuntimeManager.PlayOneShot(sound);
        }
    }
}