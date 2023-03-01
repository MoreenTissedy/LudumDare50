using UnityEngine;
using UnityEngine.Serialization;

namespace CauldronCodebase

{
    [RequireComponent(typeof (Camera))]
    public class UIHideOnPause: MonoBehaviour
    {
        [FormerlySerializedAs("camera")] [SerializeField] private Camera mainCamera;

        private void OnValidate()
        {
            if (!mainCamera)
            {
                mainCamera = GetComponent<Camera>();
            }
        }

        private void Start()
        {
            if (GameLoader.IsMenuOpen())
            {
                mainCamera.enabled = false;
            }

            GameLoader.GamePaused += OnGamePaused;
        }

        private void OnGamePaused(bool pause)
        {
            mainCamera.enabled = !pause;
        }

        private void OnDestroy()
        {
            GameLoader.GamePaused -= OnGamePaused;
        }
    }
}