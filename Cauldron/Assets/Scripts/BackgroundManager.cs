using UnityEngine;
using Zenject;

namespace CauldronCodebase
{
    public class BackgroundManager : MonoBehaviour
    {
        public GameObject[] backgrounds;

        [Inject] private EndingsProvider endings;

        private void Start()
        {
            Instantiate(backgrounds[endings.GetUnlockedEndingsCount() % backgrounds.Length], transform);
        }
    }
}