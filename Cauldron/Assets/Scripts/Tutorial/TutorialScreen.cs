using Cysharp.Threading.Tasks;
using UnityEngine;
using Universal;
using Zenject;

namespace CauldronCodebase
{
    public class TutorialScreen : MonoBehaviour
    {
        [SerializeField] private OverlayLayer overlayLayer;
        
        [SerializeField] private FlexibleButton rejectButton;
        [SerializeField] private FlexibleButton acceptButton;
        [SerializeField] private GameObject dialogButtonsRoot;
        
        [SerializeField] private FlexibleButton nextButton;
        [SerializeField] private ScrollTooltip scrollTooltip;

        [Inject] private OverlayManager overlayManager;

        public async UniTask Show(string text)
        {
            bool clicked = false;
            overlayManager.AddLayer(overlayLayer);
            await scrollTooltip.Open(text);

            dialogButtonsRoot.SetActive(false);
            nextButton.gameObject.SetActive(true);
            nextButton.OnClick += () => clicked = true;
            await UniTask.WaitUntil(() => clicked);

            scrollTooltip.Close();
            nextButton.RemoveAllSubscriptions();
            overlayManager.RemoveLayer(overlayLayer);
        }

        public async UniTask<bool> ShowAsDialog(string text)
        {
            bool accepted = false;
            bool rejected = false;
            acceptButton.OnClick += () => accepted = true;
            rejectButton.OnClick += () => rejected = true;
            dialogButtonsRoot.SetActive(true);
            nextButton.gameObject.SetActive(false);

            overlayManager.AddLayer(overlayLayer);
            await scrollTooltip.Open(text);
            await UniTask.WaitUntil(() => accepted || rejected);

            acceptButton.RemoveAllSubscriptions();
            rejectButton.RemoveAllSubscriptions();
            scrollTooltip.Close();
            dialogButtonsRoot.SetActive(false);
            overlayManager.RemoveLayer(overlayLayer);

            return accepted;
        }
    }
}