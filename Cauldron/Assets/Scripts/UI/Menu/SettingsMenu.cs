using EasyLoc;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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
        
        [Header("Pointer speed 4 gamepad")]
        [SerializeField] private Slider pointerSpeed;
        const float pointerSpeedMaxValue = 2000;
        const float pointerSpeedMinValue = 300;

        [Header("Reset data")] 
        [SerializeField] private MainMenu mainMenu;

        [SerializeField] private FlexibleButton openResetButton;
        [SerializeField] private GameObject dialogueReset;
        [SerializeField] private Button acceptResetButton;
        [SerializeField] private Button declineResetButton;

        [Header("Other")]
        [SerializeField] private FlexibleButton closeSettingsButton;

        [Header("Fade")]
        [SerializeField] [Range(0f, 1f)] private float fadeInTargetAlpha;
        [Inject] private FadeController fadeController;

        [Inject] private LocalizationTool locTool;
        [Inject] private PlayerProgressProvider progressProvider;
        [Inject] private CameraAdapt cameraAdaptation;
        [Inject] private VirtualMouseInput virtualMouse;
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
            LoadAutoCookingMode();
            LoadPointerSpeed();
            language.onValueChanged.AddListener(ChangeLanguage);
            music.onValueChanged.AddListener((x) => ChangeVolume("Music", x));
            sounds.onValueChanged.AddListener(x => ChangeVolume("SFX", x));
            pointerSpeed.onValueChanged.AddListener(ChangePointerSpeed);
            resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
            toggleFullscreen.onValueChanged.AddListener(ChangeFullscreenMode);
            autoCooking.onValueChanged.AddListener(ChangeAutoCooking);
            openResetButton.OnClick += OpenResetDialogue;
            closeSettingsButton.OnClick += Close;
            acceptResetButton.onClick.AddListener(ResetGameData);
            declineResetButton.onClick.AddListener(CloseResetDialogue);
        }

        private void LoadPointerSpeed()
        {
            pointerSpeed.value = PlayerPrefs.HasKey(PrefKeys.PointerSpeed) 
                ? GetValueFromRealSpeed(PlayerPrefs.GetInt(PrefKeys.PointerSpeed)) 
                : GetValueFromRealSpeed(virtualMouse.DefaultSpeed);

            float GetValueFromRealSpeed(float realSpeed)
            {
                return (realSpeed - pointerSpeedMinValue)/(pointerSpeedMaxValue - pointerSpeedMinValue);
            }
        }

        private void ChangePointerSpeed(float value)
        {
            int realValue = (int)Mathf.Lerp(pointerSpeedMinValue, pointerSpeedMaxValue, value);
            
            PlayerPrefs.SetInt(PrefKeys.PointerSpeed, realValue);
            virtualMouse.cursorSpeed = realValue;
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
            LoadResolutionDropdown();
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

        private void ChangeVolume(string vca, float value, float max = 1)
        {
            RuntimeManager.GetVCA($"vca:/{vca}").setVolume(Mathf.Lerp(0, max, value));
            UpdateSliderLabel(vca, value);
            PlayerPrefs.SetFloat(PrefKeys.MusicValueSettings, music.value);
            PlayerPrefs.SetFloat(PrefKeys.SoundsValueSettings, sounds.value);
        }

        private void LoadResolutionDropdown()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            
            int setResolutionIndex = -1;

            for (var index = 0; index < resolutions.Length; index++)
            {
                var res = resolutions[index];
                options.Add(res.ToString());
                if (Screen.currentResolution.ToString() == res.ToString())
                {
                    setResolutionIndex = index;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = setResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private async void ChangeResolution(int resIndex)
        {
            if (resIndex == 0)
            {
                return;
            }
            Resolution newResolution = resolutions[resIndex-1];
            Screen.SetResolution(newResolution.width, newResolution.height, fullscreenMode);

            await UniTask.NextFrame();
            cameraAdaptation.Rebuild();
        }

        private async void ChangeFullscreenMode(bool set)
        {
            fullscreenMode = set;
            PlayerPrefs.SetInt(PrefKeys.FullscreenModeSettings, fullscreenMode ? 1 : 0);
            Screen.fullScreen = fullscreenMode;
            
            if (set)
            {
                var newDisplay = Display.displays[cameraAdaptation.Display];
                Screen.SetResolution(newDisplay.systemWidth, newDisplay.systemHeight, true);
                LoadResolutionDropdown();
            }
            await UniTask.NextFrame();
            cameraAdaptation.Rebuild();
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
            else
            {
                fullscreenMode = true;
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
            sounds.value = PlayerPrefs.HasKey(PrefKeys.SoundsValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.SoundsValueSettings) : 0.8f;
            UpdateSliderLabel("SFX", sounds.value);
            music.value = PlayerPrefs.HasKey(PrefKeys.MusicValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.MusicValueSettings) : 0.8f;
            UpdateSliderLabel("Music", music.value);
        }

        private void LoadVolumeValues()
        {
            LoadSlidersValues();
        }

        private void ResetGameData()
        {
            mainMenu.ResetGameData();
            CloseResetDialogue();
        }
        
        private void LoadAutoCookingMode()
        {
            if (progressProvider.IsAutoCookingUnlocked)
            {
                OpenAutoCooking();
            }
            else
            {
                CloseAutoCooking();
            }

            if (PlayerPrefs.HasKey(PrefKeys.AutoCooking))
            {
                autoCookingMode = PlayerPrefs.GetInt(PrefKeys.AutoCooking) == 1;
                autoCooking.isOn = autoCookingMode;
            }
        }
        
        private void OpenAutoCooking()
        {
            autoCookingObject.gameObject.SetActive(true);
        }

        private void CloseAutoCooking()
        {
            autoCookingObject.gameObject.SetActive(false);
        }
    }
}