using EasyLoc;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Music and sounds")] 
        [SerializeField] private Slider music;
        [SerializeField] private Slider sounds;

        [SerializeField] private Button translate;

        [Inject] private LocalizationTool locTool;
        [SerializeField] private TextMeshProUGUI musicLabel;
        [SerializeField] private TextMeshProUGUI soundsLabel;

        [Header("Resolution")] 
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        [Header("Toggle Fullscreen")] 
        [SerializeField] private Toggle toggleFullscreen;

        [Header("Reset data")] 
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private Button openResetButton;
        [SerializeField] private GameObject dialougeReset;
        [SerializeField] private Button acceptResetButton;
        [SerializeField] private Button declineResetButton;
        
        [Header("Other")]
        [SerializeField] private Button closeSettingsButton;

        private bool fullscreenMode;
        
        private void OnValidate()
        {
            if (!mainMenu) mainMenu = FindObjectOfType<MainMenu>();
        }

        private void Start()
        {
            LoadVolumeValues();
            LoadResolution();
            Close();
            translate.onClick.AddListener(() => locTool.LoadLanguage(locTool.loadedLanguage == Language.EN ? Language.RU : Language.EN));
            music.onValueChanged.AddListener((x) => ChangeVolume("Music", x));
            sounds.onValueChanged.AddListener(x => ChangeVolume("SFX", x));
            resolutionDropdown.onValueChanged.AddListener(x => ChangeResolution(x));
            toggleFullscreen.onValueChanged.AddListener(x => ChangeFullscreenMode(x));
            openResetButton.onClick.AddListener(OpenResetDialogue);
            closeSettingsButton.onClick.AddListener(Close);
            acceptResetButton.onClick.AddListener(ResetGameData);
            declineResetButton.onClick.AddListener(CloseResetDialogue);
        }

        private void LoadResolution()
        {
            LoadFullscreenMode();
            LoadResolutionDropdown();
            if (PlayerPrefs.HasKey(PrefKeys.ResolutionSettings))
            {
                int newResolution = PlayerPrefs.GetInt(PrefKeys.ResolutionSettings);
                Screen.SetResolution(resolutions[newResolution].width, resolutions[newResolution].height, fullscreenMode);
            }
            else
            {   //Default screen resolution
                Screen.SetResolution(1920, 1080, true);
            }
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        private void OpenResetDialogue()
        {
            dialougeReset.SetActive(true);
        }
        
        private void CloseResetDialogue()
        {
            dialougeReset.SetActive(false);
        }

        private void LoadResolutionDropdown()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            int setResolutionIndex = 0;

            foreach (var res in resolutions)
            {
                options.Add(res.width + " x " + res.height);
                if (res.width == Screen.width && res.height == Screen.height)
                {
                    setResolutionIndex = currentResolutionIndex;
                }
                currentResolutionIndex++;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = setResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void ChangeResolution(int resIndex)
        {
            Resolution newResolution = resolutions[resIndex];
            Screen.SetResolution(newResolution.width, newResolution.height, fullscreenMode);
            PlayerPrefs.SetInt(PrefKeys.ResolutionSettings, resIndex);
        }

        private void ChangeVolume(string vca, float value, float max = 1)
        {
            RuntimeManager.GetVCA($"vca:/{vca}").setVolume(Mathf.Lerp(0, max, value));
            UpdateSliderLabel(vca, value);
            PlayerPrefs.SetFloat(PrefKeys.MusicValueSettings, music.value);
            PlayerPrefs.SetFloat(PrefKeys.SoundsValueSettings, sounds.value);
        }

        private void ChangeFullscreenMode(bool set)
        {
            fullscreenMode = set;
            PlayerPrefs.SetInt(PrefKeys.FullscreenModeSettings, fullscreenMode ? 1 : 0);
            Screen.fullScreen = fullscreenMode;
        }

        private void LoadFullscreenMode()
        {
            if (PlayerPrefs.HasKey(PrefKeys.FullscreenModeSettings))
            {
                fullscreenMode = PlayerPrefs.GetInt(PrefKeys.FullscreenModeSettings) == 1;
                toggleFullscreen.isOn = fullscreenMode;
            }
        }

        private void UpdateSliderLabel(string vca, float value)
        {
            string labelValue = Mathf.RoundToInt(value * 100) + "%";
            switch (vca)
            {
                case "Music":
                    musicLabel.SetText(labelValue);
                    break;
                case "SFX":
                    soundsLabel.SetText(labelValue);
                    break;
            }
        }

        private void LoadSlidersValues()
        {
            sounds.value = PlayerPrefs.HasKey(PrefKeys.SoundsValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.SoundsValueSettings) : 1f;
            UpdateSliderLabel("SFX", sounds.value);
            music.value = PlayerPrefs.HasKey(PrefKeys.MusicValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.MusicValueSettings) : 1f;
            UpdateSliderLabel("Music", music.value);
        }

        private void LoadVolumeValues()
        {
            LoadSlidersValues();
            RuntimeManager.GetVCA("vca:/Music").setVolume(music.value);
            RuntimeManager.GetVCA("vca:/SFX").setVolume(sounds.value);
        }

        private void ResetGameData()
        {
            mainMenu.ResetGameData();
            CloseResetDialogue();
        }
    }
}