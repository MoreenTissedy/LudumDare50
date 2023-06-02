using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Slider music;
        [SerializeField] private Slider sounds;
        [SerializeField] private TextMeshProUGUI musicLabel;
        [SerializeField] private TextMeshProUGUI soundsLabel;
        private void Start()
        {
            LoadValues();
            Close();
            music.onValueChanged.AddListener((x) => ChangeVolume("Music", x));
            sounds.onValueChanged.AddListener(x => ChangeVolume("SFX", x));
            
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
            UpdateLabel(vca, value);
            SaveValues();
        }

        private void UpdateLabel(string vca, float value)
        {
            string labelValue = Mathf.RoundToInt(Mathf.Lerp(0, 1, value) * 100) + "%";
            switch (vca)
            {
                case "Music": musicLabel.SetText(labelValue);
                    break;
                case "SFX": soundsLabel.SetText(labelValue);
                    break;
            }
        }

        private void SaveValues()
        {
            PlayerPrefs.SetFloat(PrefKeys.MusicValue, music.value);
            PlayerPrefs.SetFloat(PrefKeys.SoundsValue, sounds.value);
        }
        
        private void LoadValues()
        {
            sounds.value = PlayerPrefs.HasKey(PrefKeys.SoundsValue) ? PlayerPrefs.GetFloat(PrefKeys.SoundsValue) : 1f;
            UpdateLabel("SFX", sounds.value);
            music.value = PlayerPrefs.HasKey(PrefKeys.MusicValue) ? PlayerPrefs.GetFloat(PrefKeys.MusicValue) : 1f;
            UpdateLabel("Music", music.value);
        }
        
    }
}