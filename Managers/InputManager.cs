using Microsoft.Xna.Framework.Input;

namespace Program
{
    public static class InputManager
    {
        // Each [keyCode][0] = isPressed, [keyCode][1] = justReleased
        public static bool[][] KeysState = new bool[256][];  // 256 to cover most key codes
        public static MouseForm MouseState = new MouseForm();

        public static void Init()
        {
            // Initialize 2D array
            for (int i = 0; i < KeysState.Length; i++)
                KeysState[i] = new bool[2];
        }
        public static void Update()
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

        }
    }
    public class MouseForm
    {
        
    }
}
