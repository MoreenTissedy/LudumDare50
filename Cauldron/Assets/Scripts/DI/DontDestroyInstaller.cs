using EasyLoc;
using FMODUnity;
using nn.account;
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
        [SerializeField] private GameFXManager fxManager;
        [SerializeField] private GameObject cursorFx;

        private MilestoneProvider milestoneProvider;
        private VillagerFamiliarityChecker visitorsProvider;

        public override void InstallBindings()
        {
#if UNITY_SWITCH
            MountSaveRomForSwitch();
            PlayerPrefsService.Load();
#endif
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

            milestoneProvider = new MilestoneProvider();
            Container.Bind<MilestoneProvider>().FromInstance(milestoneProvider).AsSingle();

            visitorsProvider = new VillagerFamiliarityChecker();
            Container.Bind<VillagerFamiliarityChecker>().FromInstance(visitorsProvider).AsSingle();

            Container.Bind<FadeController>().FromComponentInNewPrefab(fadeController).AsSingle();
            Container.Bind<DataPersistenceManager>().FromComponentInNewPrefab(dataPersistenceManager).AsSingle()
                .NonLazy();
            Container.Bind<GameFXManager>().FromComponentInNewPrefab(fxManager).AsSingle();

            Container.Bind<SoundManager>().FromInstance(soundManager).NonLazy();
            Container.Bind<InputManager>().FromInstance(new InputManager(cursorFx, virtualMouseInput)).AsSingle()
                .NonLazy();
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
            if (PlayerPrefsService.HasKey(PrefKeys.PointerSpeed))
            {
                virtualMouseInput.cursorSpeed = PlayerPrefsService.GetInt(PrefKeys.PointerSpeed);
            }
            else
            {
                virtualMouseInput.cursorSpeed = virtualMouseInput.DefaultSpeed;
            }
        }

        private void SetSoundVolume()
        {
            var soundVolume = PlayerPrefsService.HasKey(PrefKeys.SoundsValueSettings)
                ? PlayerPrefsService.GetFloat(PrefKeys.SoundsValueSettings)
                : 0.8f;
            var musicVolume = PlayerPrefsService.HasKey(PrefKeys.MusicValueSettings)
                ? PlayerPrefsService.GetFloat(PrefKeys.MusicValueSettings)
                : 0.8f;
            RuntimeManager.GetVCA("vca:/Music").setVolume(musicVolume);
            RuntimeManager.GetVCA("vca:/SFX").setVolume(soundVolume);
        }

        public static void MountSaveRomForSwitch()
        {
            nn.account.Account.Initialize();
            nn.account.UserHandle userHandle = new nn.account.UserHandle();

            if (!nn.account.Account.TryOpenPreselectedUser(ref userHandle))
            {
                nn.Nn.Abort("Failed to open preselected user.");
            }

            nn.account.Uid userId = Uid.Invalid;
            nn.Result result = nn.account.Account.GetUserId(ref userId, userHandle);
            result.abortUnlessSuccess();
            result = nn.fs.SaveData.Mount("Saves", userId);
        }
    }
}