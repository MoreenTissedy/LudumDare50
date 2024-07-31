using System.Linq;
using EasyLoc;
using FMODUnity;
using UnityEngine;
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
        [SerializeField] private RecipeProvider recipeProvider;
        [SerializeField] private EndingsProvider endings;

        [SerializeField] private SoundManager soundManager;
        [SerializeField] private FadeController fadeController;

        public override void InstallBindings()
        {
            Camera mainCameraScript = Container.InstantiatePrefab(mainCamera).GetComponent<Camera>();
            Container.Bind<Camera>().FromInstance(mainCameraScript).AsSingle();
            Container.Bind<CatTipsProvider>().FromInstance(catTipsProvider).AsSingle();
            Container.Bind<SODictionary>().FromInstance(soDictionary).AsSingle();
            soDictionary.LoadDictionary();

            Container.Bind<MainSettings>().FromInstance(mainSettings).AsSingle().NonLazy();
            
            Container.Bind<PlayerProgressProvider>().FromNew().AsSingle().NonLazy();
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle().NonLazy();
            recipeProvider.Load();            
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
            Container.Bind<DataPersistenceManager>().FromComponentInNewPrefab(dataPersistenceManager).AsSingle().NonLazy();
            Container.Bind<SoundManager>().FromInstance(soundManager).NonLazy();
            Container.Bind<FadeController>().FromComponentInNewPrefab(fadeController).AsSingle();
            Container.Bind<LocalizationTool>().FromNew().AsSingle();
            
            SetInitialResolution();
            SetSoundVolume();
        }

        private static void SetInitialResolution()
        {
            bool fullscreenMode = true;
            if (PlayerPrefs.HasKey(PrefKeys.FullscreenModeSettings))
            {
                fullscreenMode = PlayerPrefs.GetInt(PrefKeys.FullscreenModeSettings) == 1;
            }

            if (PlayerPrefs.HasKey(PrefKeys.ResolutionSettings))
            {
                var resolutions = Screen.resolutions;
                int newResolution = PlayerPrefs.GetInt(PrefKeys.ResolutionSettings);
                if (resolutions.Length > newResolution)
                {
                    Resolution savedResolution = resolutions[newResolution];
                    Screen.SetResolution(savedResolution.width, savedResolution.height, fullscreenMode, savedResolution.refreshRate);
                    return;
                }
                PlayerPrefs.DeleteKey(PrefKeys.ResolutionSettings);
            }
            var chosenResolution = GetOptimalResolution(1080f / 1920);
            Screen.SetResolution(chosenResolution.width, chosenResolution.height, fullscreenMode);
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