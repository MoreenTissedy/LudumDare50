using UnityEngine;
using Universal;

namespace Buttons
{
    public class SetActiveAnimation: AnimatedButtonComponent
    {
        public bool invert = false;
        public GameObject activeOnSelected;

        private void Start()
        {
            activeOnSelected.SetActive(invert);
        }

        public override void Select()
        {
            activeOnSelected.SetActive(!invert);
        }

        public override void Unselect()
        {
            activeOnSelected.SetActive(invert);
        }

        public override void Activate()
        {
            //Unselect();
        }

        public override void ChangeInteractive(bool isInteractive)
        {
            activeOnSelected.SetActive(invert);
        }
    }
}