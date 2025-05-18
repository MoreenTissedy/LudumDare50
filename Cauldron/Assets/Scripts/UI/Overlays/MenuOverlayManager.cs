using UnityEngine;

namespace CauldronCodebase
{
    public class MenuOverlayManager: OverlayManager
    {
        [SerializeField] private AuthorsMenu authorsMenu;
        [SerializeField] private SettingsMenu settingsMenu;

        public void OpenSettings()
        {
            settingsMenu.Open();
            AddLayer(settingsMenu.mainLayer);
        }

        public void OpenAuthors()
        {
            authorsMenu.Open();
            AddLayer(authorsMenu.GetComponent<OverlayLayer>());
        }
    }
}