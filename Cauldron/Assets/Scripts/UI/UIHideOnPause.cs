using UnityEngine;

namespace CauldronCodebase

{
    [RequireComponent(typeof (Camera))]
    public class UIHideOnPause: MonoBehaviour
    {
        [SerializeField] private Camera camera;

        private void OnValidate()
        {
            if (!camera)
            {
                camera = GetComponent<Camera>();
            }
        }

        private void Start()
        {
            if (GameLoader.IsMenuOpen())
            {
                camera.enabled = false;
            }

            GameLoader.GamePaused += OnGamePaused;
        }

        private void OnGamePaused(bool pause)
        {
            camera.enabled = !pause;
        }

        private void OnDestroy()
        {
            GameLoader.GamePaused -= OnGamePaused;
        }
    }
}