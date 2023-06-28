using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace CauldronCodebase
{
    public enum Sounds
    {
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
        NightCardEnter,
        NightCardExit,
        StartSpark,
        StartDay,
        EndDay,
        GameEnd,
        EndingUnlock,
        SpecialEndingUnlock
    }

    public enum Music
    {
        Menu,
        Location1,
        Location2
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
        public string[] soundKeys;
        public string[] musicKeys;
        public EventReference[] sounds;
        public EventReference[] musics;
        public VisitorSounds defaultVisitorSounds;
        public CatSounds catSounds;

        private EventInstance currentMusic;
        private EventInstance bubbling;

        private void OnValidate()
        {
            if (EnumHasChanged<Sounds>(soundKeys))
            {
                RearrangeArrays<Sounds>(ref soundKeys, ref sounds);
            }
            if (EnumHasChanged<Music>(musicKeys))
            {
                RearrangeArrays<Music>(ref musicKeys, ref musics);
            }
        }

        private bool EnumHasChanged<T>(in string[] array)
        {
            var enumKeys = Enum.GetNames(typeof(T));
            return enumKeys != array;
        }

        private void RearrangeArrays<T>(ref string[] keys, ref EventReference[] values)
        {
            int enumSize = Enum.GetValues(typeof(T)).Length;
            Dictionary<string, EventReference> dict = new Dictionary<string, EventReference>(enumSize);
            for (int i = 0; i < keys.Length; i++)
            {
                dict.Add(keys[i], values[i]);
            }
            keys = Enum.GetNames(typeof(T));
            var soundList = new List<EventReference>(enumSize);
            foreach (var key in keys)
            {
                dict.TryGetValue(key, out var value);
                soundList.Add(value);
            }
            values = soundList.ToArray();
        }

        public void SetMusic(Music music, bool withBubbling)
        {
            if (!bubbling.isValid())
            {
                bubbling = RuntimeManager.CreateInstance(sounds[(int)Sounds.Bubbling]);
            }
            if (currentMusic.isValid())
            {
                currentMusic.stop(STOP_MODE.ALLOWFADEOUT);
            }
            currentMusic = RuntimeManager.CreateInstance(musics[(int)music]);
            currentMusic.start();
            if (withBubbling)
            {
                bubbling.start();
            }
            else
            {
                bubbling.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }

        public void StopMusic()
        {
            if (bubbling.isValid())
            {
                bubbling.stop(STOP_MODE.ALLOWFADEOUT);
            }

            if (currentMusic.isValid())
            {
                currentMusic.stop(STOP_MODE.ALLOWFADEOUT);
            }
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