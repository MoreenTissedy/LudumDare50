using Save;
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
        
        [SerializeField] private SoundManager soundManager;
        [SerializeField] private FadeController fadeController;
        public override void InstallBindings()
        {
            Camera mainCameraScript = Container.InstantiatePrefab(mainCamera).GetComponent<Camera>();
            Container.Bind<Camera>().FromInstance(mainCameraScript).AsSingle();
            Container.Bind<SODictionary>().FromInstance(soDictionary).AsSingle();
            Container.Bind<FadeController>().FromInstance(fadeController).AsSingle();
            soDictionary.LoadDictionary();

            Container.Bind<MainSettings>().FromInstance(mainSettings).AsSingle().NonLazy();
            
            DataPersistenceManager dataPersistenceScript = Container.InstantiatePrefab(dataPersistenceManager).GetComponent<DataPersistenceManager>();
            Container.Bind<DataPersistenceManager>().FromInstance(dataPersistenceScript).AsSingle().NonLazy();
            Container.Bind<SoundManager>().FromInstance(soundManager).NonLazy();
        }
    }
}