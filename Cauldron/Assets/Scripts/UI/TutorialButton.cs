using UnityEngine;
using UnityEngine.EventSystems;
using Universal;

namespace CauldronCodebase
{
    public class TutorialButton : GrowOnMouseEnter
    {
        public Tutorial tutorialScript;
        public bool bookTutorial;
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (bookTutorial)
                tutorialScript.ShowOnBook();
            else
            {
                tutorialScript.ShowGameplay();
            }
        }
    }
}