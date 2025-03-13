using UnityEngine;

namespace Buttons
{
    public class SelectOnEnable: MonoBehaviour
    {
        //add controller check
        private void OnEnable()
        {
            GetComponent<ISelectable>()?.Select();
        }

        private void OnDisable()
        {
            GetComponent<ISelectable>().Unselect();
        }
    }
}