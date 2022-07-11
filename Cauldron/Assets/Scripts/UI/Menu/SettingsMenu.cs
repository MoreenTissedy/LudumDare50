using System;
using UnityEngine;

namespace CauldronCodebase
{
    public class SettingsMenu : MonoBehaviour
    {
        private void Start()
        {
            Close();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}