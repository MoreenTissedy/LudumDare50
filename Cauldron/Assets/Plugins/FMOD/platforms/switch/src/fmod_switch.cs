#if UNITY_SWITCH
using System.Runtime.InteropServices;

namespace FMOD
{
    public static class Switch
    {
        public static RESULT SetHTCSEnabled(bool enabled)
        {
            return FMOD_Switch_SetHTCSEnabled(enabled);
        }

#region importfunctions
        [DllImport(VERSION.dll)]
        private static extern RESULT FMOD_Switch_SetHTCSEnabled(bool enabled);
#endregion
    }
}
#endif
