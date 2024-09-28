using EasyLoc;
using FMODUnity;
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
        [SerializeField] private RecipeProvider recipeProvider;
        [SerializeField] private EndingsProvider endings;
        [SerializeField] private VirtualMouseInput virtualMouseInput;

        [SerializeField] private SoundManager soundManager;
        [SerializeField] private FadeController fadeController;

        private MilestoneProvider milestoneProvider;        
        private VillagerFamiliarityChecker visitorsProvider;

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
            
            Container.Bind<PlayerProgressProvider>().FromNew().AsSingle().NonLazy();
            Container.Bind<RecipeProvider>().FromInstance(recipeProvider).AsSingle().NonLazy();
            recipeProvider.Load();            

            milestoneProvider = new MilestoneProvider();
            Container.Bind<MilestoneProvider>().FromInstance(milestoneProvider).AsSingle();
            
            Container.Bind<EndingsProvider>().FromInstance(endings).AsSingle();
            
            visitorsProvider = new VillagerFamiliarityChecker();
            Container.Bind<VillagerFamiliarityChecker>().FromInstance(visitorsProvider).AsSingle();
            
            Container.Bind<DataPersistenceManager>().FromComponentInNewPrefab(dataPersistenceManager).AsSingle().NonLazy();
            Container.Bind<SoundManager>().FromInstance(soundManager).NonLazy();
            Container.Bind<FadeController>().FromComponentInNewPrefab(fadeController).AsSingle();
            Container.Bind<InputManager>().FromNew().AsSingle();
            Container.Bind<LocalizationTool>().FromNew().AsSingle();
            Container.Bind<VirtualMouseInput>().FromInstance(virtualMouseInput).AsSingle();
            
            SetPointerSpeed();
            cameraAdaptation.Rebuild();
            SetSoundVolume();
            
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
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

        private void SetSoundVolume()
        {
            var soundVolume = PlayerPrefs.HasKey(PrefKeys.SoundsValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.SoundsValueSettings) : 0.8f;
            var musicVolume = PlayerPrefs.HasKey(PrefKeys.MusicValueSettings) ? PlayerPrefs.GetFloat(PrefKeys.MusicValueSettings) : 0.8f;
            RuntimeManager.GetVCA("vca:/Music").setVolume(musicVolume);
            RuntimeManager.GetVCA("vca:/SFX").setVolume(soundVolume);
        }
    }
}