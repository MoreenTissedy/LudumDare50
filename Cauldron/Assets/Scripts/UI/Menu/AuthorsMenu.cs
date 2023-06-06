using System;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace CauldronCodebase
{
    public class AuthorsMenu : MonoBehaviour
    {
        [SerializeField] private Button closePanelButton;
        [Header("Fade In Out")]
        [SerializeField] private FadeController fade;

        private void OnValidate()
        {
            if (!fade) fade = FindObjectOfType<FadeController>(true);
        }

        private void Start()
        {
            closePanelButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            fade.FadeIn();
        }
        
        public void Close()
        {
            fade.FadeOut();
            gameObject.SetActive(false);
            
        }
        
    }
}
