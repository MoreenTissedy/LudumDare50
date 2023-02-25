using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Slider music;
        [SerializeField] private Slider sounds;
        
        private void Start()
        {
            Close();
            //0.4 as temp measure
            music.onValueChanged.AddListener((x) => ChangeVolume("Music", x, 0.4f));
            sounds.onValueChanged.AddListener(x => ChangeVolume("Sounds", x));
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void ChangeVolume(string vca, float value, float max = 1)
        {
            RuntimeManager.GetVCA($"vca:/{vca}").setVolume(Mathf.Lerp(0, max, value));
        }
    }
}