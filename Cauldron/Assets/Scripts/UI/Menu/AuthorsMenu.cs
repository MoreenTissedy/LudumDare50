using Cysharp.Threading.Tasks;
using UnityEngine;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class AuthorsMenu : MonoBehaviour
    {
        [SerializeField] private FlexibleButton closePanelButton;

        [Header("Fade")]
        [SerializeField] [Range(0f, 1f)] private float fadeInTargetAlpha;
        [Inject] private FadeController fadeController;


        private void Start()
        {
            closePanelButton.OnClick += Close;
        }

        public void Open()
        {
            gameObject.SetActive(true);
            fadeController.FadeIn(endAlpha: fadeInTargetAlpha).Forget();
        }
        
        public void Close()
        {
            gameObject.SetActive(false);
            fadeController.FadeOut().Forget();
        }
        
    }
}
