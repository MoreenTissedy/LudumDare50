using System;
using UnityEngine;

namespace Universal
{
    public class ClampResolution : MonoBehaviour
    {
        public float minAspect = 1.3f;
        public float maxAspect = 1.5f;

        public void Start()
        {
            ClampScreenResolution();
        }

        private void ClampScreenResolution()
        {
            var aspect = Screen.width * 1f / Screen.height;
            if (aspect > maxAspect)
            {
                Screen.SetResolution((int) (Screen.height * maxAspect), Screen.height, true);
            }
            else if (aspect < minAspect)
            {
                Screen.SetResolution(Screen.width, (int) (Screen.height * minAspect), true);
            }
        }
    }
}