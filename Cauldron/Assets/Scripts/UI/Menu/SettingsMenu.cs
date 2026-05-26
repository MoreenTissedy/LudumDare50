using Buttons;
using Cysharp.Threading.Tasks;
using EasyLoc;
using FMODUnity;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class SettingsMenu : MonoBehaviour
    {
        public MenuOverlayManager overlayManager;
        public OverlayLayer mainLayer;
        public OverlayLayer popupLayer;
        [Header("Music and sounds")] 
        [SerializeField] private Slider music;
        [SerializeField] private Slider sounds;

        [SerializeField] private TextMeshProUGUI musicLabel;
        [SerializeField] private TextMeshProUGUI soundsLabel;
        
        [Header("Language")]
        [SerializeField] private SelectableList language;
        [SerializeField] private FlexibleButton changeLanguage;

        [Header("Resolution")]
        [SerializeField] private SelectableList resolution;
        [SerializeField] private FlexibleButton changeResolution;

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
        [SerializeField] private FlexibleButton acceptResetButton;
        [SerializeField] private FlexibleButton declineResetButton;

        [Header("Other")]
        [SerializeField] private FlexibleButton closeSettingsButton;

        [Header("Fade")]
        [SerializeField] [Range(0f, 1f)] private float fadeInTargetAlpha;
        [Inject] private FadeController fadeController;

        [Inject] private LocalizationTool locTool;
        [Inject] private PlayerProgressProvider progressProvider;
        [Inject] private CameraAdapt cameraAdaptation;
        [Inject] private VirtualMouseInput virtualMouse;
        [Inject] private InputManager inputManager;
        
        private bool fullscreenMode;
        private bool autoCookingMode;
        private Language selectedLanguage;
        private int selectedResolutionIndex;
        
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
            language.OnValueChanged += UpdateSelectedLanguage;
            changeLanguage.OnClick += ChangeLanguageToSelected;
            resolution.OnValueChanged += UpdateSelectedResolution;
            changeResolution.OnClick += ChangeResolutionToSelected;
            music.onValueChanged.AddListener((x) => ChangeVolume("Music", x));
            sounds.onValueChanged.AddListener(x => ChangeVolume("SFX", x));
            pointerSpeed.onValueChanged.AddListener(ChangePointerSpeed);
            toggleFullscreen.onValueChanged.AddListener(ChangeFullscreenMode);
            autoCooking.onValueChanged.AddListener(ChangeAutoCooking);
            
            closeSettingsButton.OnClick += Close;
            inputManager.Controls.General.Exit.performed += (_) => Close();
            
            openResetButton.OnClick += OpenResetDialogue;
            acceptResetButton.OnClick += ResetGameData;
            declineResetButton.OnClick += CloseResetDialogue;
        }

        private void LoadPointerSpeed()
        {
            pointerSpeed.value = PlayerPrefsService.HasKey(PrefKeys.PointerSpeed) 
                ? GetValueFromRealSpeed(PlayerPrefsService.GetInt(PrefKeys.PointerSpeed)) 
                : GetValueFromRealSpeed(virtualMouse.DefaultSpeed);

            float GetValueFromRealSpeed(float realSpeed)
            {
                return (realSpeed - pointerSpeedMinValue)/(pointerSpeedMaxValue - pointerSpeedMinValue);
            }
        }

        private void ChangePointerSpeed(float value)
        {
            int realValue = (int)Mathf.Lerp(pointerSpeedMinValue, pointerSpeedMaxValue, value);
            
            PlayerPrefsService.SetInt(PrefKeys.PointerSpeed, realValue, false);
            virtualMouse.cursorSpeed = realValue;
        }

        private void LoadLanguage()
        {
            if (PlayerPrefsService.HasKey(PrefKeys.LanguageKey))
            {
                language.SetValueWithoutNotify(PlayerPrefsService.GetString(PrefKeys.LanguageKey) == Language.EN.ToString() ? 0 : 1);
            }
            else
            {
                language.SetValueWithoutNotify(0);
            }
        }

        private void UpdateSelectedLanguage(int index)
        {
            selectedLanguage = index > 0 ? Language.RU : Language.EN;
        }
        
        private async void ChangeLanguageToSelected()
        {
            if (selectedLanguage == Language.None)
            {
                return;
            }

            Language newLanguage = selectedLanguage;
            selectedLanguage = Language.None;
            PlayerPrefsService.SetString(PrefKeys.LanguageKey, newLanguage.ToString(), true);

            await fadeController.FadeIn(
                endAlpha: 0.5f, 
                duration: 0.3f, 
                mode: FadeMode.OverPopup, 
                blockInput: overlayManager,
                showWait: true);
            await locTool.LoadLanguage(newLanguage);
            await fadeController.FadeOut(
                duration: 0.3f, 
                unblockInput: overlayManager);
        }

        private void ChangeResolutionToSelected()
        {
            ChangeResolution(selectedResolutionIndex);
        }

        private void UpdateSelectedResolution(int index)
        {
            selectedResolutionIndex = index;
        }

        private void LoadResolution()
        {
            //LoadFullscreenMode();
            LoadResolutionSelector();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            fadeController.FadeIn(endAlpha: fadeInTargetAlpha).Forget();
        }

        public void Close()
        {
            PlayerPrefsService.Save();
            gameObject.SetActive(false);
            fadeController.FadeOut().Forget();
            overlayManager.RemoveLayer(mainLayer);
        }

        private void OpenResetDialogue()
        {
            dialogueReset.SetActive(true);
            overlayManager.AddLayer(popupLayer);
        }
        
        private void CloseResetDialogue()
        {
            dialogueReset.SetActive(false);
            overlayManager.RemoveLayer(popupLayer);
        }

        private void ChangeVolume(string vca, float value, float max = 1)
        {
            RuntimeManager.GetVCA($"vca:/{vca}").setVolume(Mathf.Lerp(0, max, value));
            UpdateSliderLabel(vca, value);
            PlayerPrefsService.SetFloat(PrefKeys.MusicValueSettings, music.value, false);
            PlayerPrefsService.SetFloat(PrefKeys.SoundsValueSettings, sounds.value, false);
        }

        private void LoadResolutionSelector()
        {
            resolutions = Screen.resolutions;
            resolution.Values = new string[resolutions.Length];

            int currentResolutionIndex = -1;

            for (var index = 0; index < resolutions.Length; index++)
            {
                var res = resolutions[index];
                resolution.Values[index] = res.ToString();
                if (Screen.currentResolution.ToString() == res.ToString())
                {
                    currentResolutionIndex = index;
                }
            }

            resolution.SetValueWithoutNotify(currentResolutionIndex);
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
            PlayerPrefsService.SetInt(PrefKeys.FullscreenModeSettings, fullscreenMode ? 1 : 0, false);
            Screen.fullScreen = fullscreenMode;
            
            if (set)
            {
                var newDisplay = Display.displays[cameraAdaptation.Display];
                Screen.SetResolution(newDisplay.systemWidth, newDisplay.systemHeight, true);
                //LoadResolutionDropdown();
            }
            await UniTask.NextFrame();
            cameraAdaptation.Rebuild();
        }

        private void ChangeAutoCooking(bool set)
        {
            autoCookingMode = set;
            PlayerPrefsService.SetInt(PrefKeys.AutoCooking, autoCookingMode ? 1 : 0, false);
        }

        private void LoadFullscreenMode()
        {
            if (PlayerPrefsService.HasKey(PrefKeys.FullscreenModeSettings))
            {
                fullscreenMode = PlayerPrefsService.GetInt(PrefKeys.FullscreenModeSettings) == 1;
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
            sounds.value = PlayerPrefsService.HasKey(PrefKeys.SoundsValueSettings) ? PlayerPrefsService.GetFloat(PrefKeys.SoundsValueSettings) : 0.8f;
            UpdateSliderLabel("SFX", sounds.value);
            music.value = PlayerPrefsService.HasKey(PrefKeys.MusicValueSettings) ? PlayerPrefsService.GetFloat(PrefKeys.MusicValueSettings) : 0.8f;
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

            if (PlayerPrefsService.HasKey(PrefKeys.AutoCooking))
            {
                autoCookingMode = PlayerPrefsService.GetInt(PrefKeys.AutoCooking) == 1;
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

        private void OnDestroy()
        {
            openResetButton.OnClick -= OpenResetDialogue;
            closeSettingsButton.OnClick -= Close;
            acceptResetButton.OnClick -= ResetGameData;
            declineResetButton.OnClick -= CloseResetDialogue;         
        }
    }
}