namespace CauldronCodebase
{
    public enum GamepadType
    {
        None,
        XBox,
        Playstation,
        Switch,
        Unknown
    }
    public class InputManager
    {
        public readonly Controls Controls;

        public bool GamepadConnected;
        public GamepadType GamepadType;
        
        public InputManager()
        {
            Controls = new Controls();
            Controls.General.Enable();
            Controls.UI.Enable();
            
            //For tests, will get real data later
            GamepadConnected = true;
            GamepadType = GamepadType.Playstation;
        }
    }
}