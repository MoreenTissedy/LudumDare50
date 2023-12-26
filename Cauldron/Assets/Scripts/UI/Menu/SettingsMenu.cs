using EasyLoc;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Music and sounds")] 
        [SerializeField] private Slider music;
        [SerializeField] private Slider sounds;

        [SerializeField] private TextMeshProUGUI musicLabel;
        [SerializeField] private TextMeshProUGUI soundsLabel;
        
        [Header("Language")]
        [SerializeField] private TMP_Dropdown language;

        [Header("Resolution")] 
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        private Resolution[] resolutions;

        [Header("Toggle Fullscreen")] 
        [SerializeField] private Toggle toggleFullscreen;

        [Header("Toggle AutoCooking")] 
        [SerializeField] private Toggle autoCooking;
        [SerializeField] private GameObject autoCookingObject;

        [Header("Reset data")] 
        [SerializeField] private MainMenu mainMenu;

        [SerializeField] private AnimatedButton openResetButton;
        [SerializeField] private GameObject dialogueReset;
        [SerializeField] private Button acceptResetButton;
        [SerializeField] private Button declineResetButton;

        [Header("Other")]
        [SerializeField] private AnimatedButton closeSettingsButton;

        [Header("Fade")]
        [SerializeField] [Range(0f, 1f)] private float fadeInTargetAlpha;
        [Inject] private FadeController fadeController;

        [Inject] private LocalizationTool locTool;
        private bool fullscreenMode;
        private bool autoCookingMode;
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (!mainMenu) mainMenu = FindObjectOfType<MainMenu>();
        }
        #endif

        private void Start()
        {
            LoadVolumeValues();
            LoadResolution();
            LoadLanguage();
            language.onValueChanged.AddListener(ChangeLanguage);
            music.onValueChanged.AddListener((x) => ChangeVolume("Music", x));
            sounds.onValueChanged.AddListener(x => ChangeVolume("SFX", x));
            resolutionDropdown.onValueChanged.AddListener(x => ChangeResolution(x));
            toggleFullscreen.onValueChanged.AddListener(x => ChangeFullscreenMode(x));
            autoCooking.onValueChanged.AddListener(x => ChangeAutoCooking(x));
            openResetButton.OnClick += OpenResetDialogue;
            closeSettingsButton.OnClick += Close;
            acceptResetButton.onClick.AddListener(ResetGameData);
            declineResetButton.onClick.AddListener(CloseResetDialogue);
        }

        private void LoadLanguage()
        {
            if (PlayerPrefs.HasKey(PrefKeys.LanguageKey))
            {
                language.SetValueWithoutNotify(PlayerPrefs.GetString(PrefKeys.LanguageKey) == Language.EN.ToString() ? 0 : 1);
            }
        }

        private void ChangeLanguage(int index)
        {
            var newLanguage = index > 0 ? Language.RU : Language.EN;
            PlayerPrefs.SetString(PrefKeys.LanguageKey, newLanguage.ToString());
            locTool.LoadLanguage(newLanguage);
        }

        private void LoadResolution()
        {
            LoadFullscreenMode();
            LoadAutoCookingMode();
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
            fadeController.FadeIn(endAlpha: fadeInTargetAlpha).Forget();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            fadeController.FadeOut().Forget();
        }

        private void OpenResetDialogue()
        {
            dialogueReset.SetActive(true);
        }
        
        private void CloseResetDialogue()
        {
            dialogueReset.SetActive(false);
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

        private void ChangeAutoCooking(bool set)
        {
            autoCookingMode = set;
            PlayerPrefs.SetInt(PrefKeys.AutoCooking, autoCookingMode ? 1 : 0);
        }

        private void LoadFullscreenMode()
        {
            if (PlayerPrefs.HasKey(PrefKeys.FullscreenModeSettings))
            {
                fullscreenMode = PlayerPrefs.GetInt(PrefKeys.FullscreenModeSettings) == 1;
                toggleFullscreen.isOn = fullscreenMode;
            }
        }

        private void LoadAutoCookingMode()
        {
            if (PlayerPrefs.GetInt(PrefKeys.IsOpenAutoCooking) == 1) 
                OpenAutoCooking();
            else
                CloseAutoCooking();

            if (PlayerPrefs.HasKey(PrefKeys.AutoCooking))
            {
                autoCookingMode = PlayerPrefs.GetInt(PrefKeys.AutoCooking) == 1;
                autoCooking.isOn = autoCookingMode;
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
        
        private void OpenAutoCooking() => 
            autoCookingObject.gameObject.SetActive(true);

        private void CloseAutoCooking() => 
            autoCookingObject.gameObject.SetActive(false);
    }
}