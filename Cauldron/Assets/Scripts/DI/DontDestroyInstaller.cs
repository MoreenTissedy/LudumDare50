using System.Linq;
using EasyLoc;
using FMODUnity;
using Save;
using UnityEngine;
using Universal;
using UnityEngine.InputSystem.UI;
using Zenject;

namespace CauldronCodebase
{
    public class DontDestroyInstaller : MonoInstaller
    {
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private MainSettings mainSettings;
        [SerializeField] private GameObject dataPersistenceManager;
        [SerializeField] private SODictionary soDictionary;
        [SerializeField] private CatTipsProvider catTipsProvider;
        [SerializeField] private VirtualMouseInput virtualMouseInput;

        [SerializeField] private SoundManager soundManager;
        [SerializeField] private FadeController fadeController;
        public override void InstallBindings()
        {
            GameObject cameraInstance = Container.InstantiatePrefab(mainCamera);
            Camera mainCameraScript = cameraInstance.GetComponent<Camera>();
            CameraAdapt cameraAdaptation = cameraInstance.GetComponent<CameraAdapt>();
            Container.Bind<CameraAdapt>().FromInstance(cameraAdaptation).AsSingle();
            Container.Bind<Camera>().FromInstance(mainCameraScript).AsSingle();
            
            Container.Bind<CatTipsProvider>().FromInstance(catTipsProvider).AsSingle();
            Container.Bind<SODictionary>().FromInstance(soDictionary).AsSingle();
            soDictionary.LoadDictionary();

            Container.Bind<MainSettings>().FromInstance(mainSettings).AsSingle().NonLazy();
            
            Container.Bind<DataPersistenceManager>().FromComponentInNewPrefab(dataPersistenceManager).AsSingle().NonLazy();
            Container.Bind<SoundManager>().FromInstance(soundManager).NonLazy();
            Container.Bind<FadeController>().FromComponentInNewPrefab(fadeController).AsSingle();
            Container.Bind<InputManager>().FromNew().AsSingle();
            Container.Bind<LocalizationTool>().FromNew().AsSingle();
            Container.Bind<VirtualMouseInput>().FromInstance(virtualMouseInput).AsSingle();
            
            SetPointerSpeed();
            SetInitialResolution(cameraAdaptation);
            SetSoundVolume();
        }

        private void SetPointerSpeed()
        {
            if (PlayerPrefs.HasKey(PrefKeys.PointerSpeed))
            {
                virtualMouseInput.cursorSpeed = PlayerPrefs.GetInt(PrefKeys.PointerSpeed);
            }
            else
            {
                virtualMouseInput.cursorSpeed = virtualMouseInput.DefaultSpeed;
            }
        }

        private static void SetInitialResolution(CameraAdapt cameraAdaptation)
        {
            bool fullscreenMode = true;
            if (PlayerPrefs.HasKey(PrefKeys.FullscreenModeSettings))
            {
                fullscreenMode = PlayerPrefs.GetInt(PrefKeys.FullscreenModeSettings) == 1;
            }

            if (PlayerPrefs.HasKey(PrefKeys.ResolutionSettings))
            {
                string newResolution = PlayerPrefs.GetString(PrefKeys.ResolutionSettings);
                foreach (Resolution resolution in Screen.resolutions)
                {
                    if (resolution.ToString() == newResolution)
                    {
                        Screen.SetResolution(resolution.width, resolution.height, fullscreenMode, resolution.refreshRate);
                        break;
                    }
                }
            }
            
            cameraAdaptation.Rebuild();
            //var chosenResolution = GetOptimalResolution(1080f / 1920);
            //Screen.SetResolution(chosenResolution.width, chosenResolution.height, fullscreenMode);
        }

        private void SetSoundVolume()
        {
            var soundVolume = PlayerPrefs.HasKey(PrefKeys.SoundsValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.SoundsValueSettings) : 0.8f;
            var musicVolume = PlayerPrefs.HasKey(PrefKeys.MusicValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.MusicValueSettings) : 0.8f;
            RuntimeManager.GetVCA("vca:/Music").setVolume(musicVolume);
            RuntimeManager.GetVCA("vca:/SFX").setVolume(soundVolume);
        }

        private static Resolution GetOptimalResolution(float aspectRatio)
        {
            var resolutions = Screen.resolutions;
            var sortedResolutions = resolutions.OrderByDescending(x => x.height);
            Resolution chosenResolution = Screen.currentResolution;
            foreach (var resolution in sortedResolutions)
            {
                var aspect = (float)resolution.height / resolution.width;
                if (Mathf.Abs(aspect - aspectRatio) < 0.01f)
                {
                    chosenResolution = resolution;
                    break;
                }
            }
            return chosenResolution;
        }
    }
}