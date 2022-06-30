using UnityEngine;

namespace CauldronCodebase
{
    public class TestNightPanel : MonoBehaviour
    {
        public NightEvent[] events;
        public NightPanel panel;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                OpenPanel();
            }
        }

        void OpenPanel()
        {
            var newList = new NightEvent[3];
            events.CopyTo(newList, 0);
            panel.OpenBookWithEvents(newList, true);
        }
    }
}