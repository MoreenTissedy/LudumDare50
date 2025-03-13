using UnityEngine;
using Universal;

namespace Buttons
{
    public class SetActiveAnimation: IAnimatedButtonComponent
    {
        public GameObject activeOnSelected;

        private void Start()
        {
            activeOnSelected.SetActive(false);
        }

        public override void Select()
        {
            activeOnSelected.SetActive(true);
        }

        public override void Unselect()
        {
            activeOnSelected.SetActive(false);
        }

        public override void Activate()
        {
            Unselect();
        }

        public override void ChangeInteractive(bool isInteractive)
        {
            activeOnSelected.SetActive(false);
        }
    }
}