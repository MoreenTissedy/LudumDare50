using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class CovenFeatureInitializer: MonoBehaviour
    {
        private GameDataHandler gameDataHandler;
        
        [Inject]
        private void Construct(GameDataHandler dataHandler)
        {
            gameDataHandler = dataHandler;
        }

        private void Start()
        {
            gameObject.SetActive(StoryTagHelper.CovenFeatureUnlocked(gameDataHandler));
        }

        [Button("Unlock feature")]
        public void Unlock()
        {
            gameObject.SetActive(true);
        }
    }
}