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

        [Header("Fade")]
        [SerializeField] [Range(0f, 1f)] private float fadeInTargetAlpha;
        [Inject] private FadeController fadeController;


        private void Start()
        {
            closePanelButton.onClick.AddListener(Close);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            fadeController.FadeIn(endAlpha: fadeInTargetAlpha);
        }
        
        public void Close()
        {
            fadeController.FadeOut();
            gameObject.SetActive(false);
            
        }
        
    }
}
