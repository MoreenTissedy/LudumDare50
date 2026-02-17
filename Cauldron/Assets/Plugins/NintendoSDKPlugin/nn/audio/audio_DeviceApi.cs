/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE 
using System.Runtime.InteropServices;

namespace nn.audio
{
    public static partial class AudioDevice
    {
        public enum AudioDeviceName
        {
            ExternalOutput = 0,
            BuiltInSpeakerOutput,
            TvOutput,
        }

#if !UNITY_SWITCH || UNITY_EDITOR
        public static bool SetOutputVolume(AudioDeviceName deviceName, float volume)
        {
            return false;
        }

        public static float GetOutputVolume(AudioDeviceName deviceName)
        {
            return 0;
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_audio_SetAudioDeviceOutputVolume")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SetOutputVolume(AudioDeviceName deviceName, float volume);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_audio_GetAudioDeviceOutputVolume")]
        public static extern float GetOutputVolume(AudioDeviceName deviceName);
#endif
    }
}
#endif
