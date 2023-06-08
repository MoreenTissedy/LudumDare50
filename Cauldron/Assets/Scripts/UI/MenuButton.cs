using UnityEngine.EventSystems;
using Universal;

namespace CauldronCodebase
{
    public class MenuButton : GrowOnMouseEnter
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (GameLoader.IsMenuOpen())
            {
                GameLoader.UnloadMenu();
            }
            else
            {
                GameLoader.LoadMenu();
            }
        }
    }
}