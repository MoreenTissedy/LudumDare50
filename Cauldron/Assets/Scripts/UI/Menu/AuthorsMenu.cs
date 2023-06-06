using System;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CauldronCodebase
{
    public class AuthorsMenu : MonoBehaviour
    {
        [SerializeField] private Button closePanelButton;
        [Header("Fade In Out")]
        [SerializeField] [Inject] private FadeController fadeController;
        

        private void Start()
        {
            closePanelButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            fadeController.FadeIn(endAlpha:0.5f);
        }
        
        public void Close()
        {
            fadeController.FadeOut(duration:1f);
            gameObject.SetActive(false);
            
        }
        
    }
}
