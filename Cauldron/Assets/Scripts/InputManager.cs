namespace CauldronCodebase
{
    public class InputManager
    {
        public readonly Controls Controls;
        
        public InputManager()
        {
            Controls = new Controls();
            Controls.General.Enable();
            Controls.UI.Enable();
            
            //debug
        }
    }
}