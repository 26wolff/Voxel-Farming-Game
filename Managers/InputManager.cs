using Microsoft.Xna.Framework.Input;

namespace Program
{
    public static class InputManager
    {
        public static bool[] Key = new bool[255];
        private static KeyboardState _currentKeyboardState;
        private static KeyboardState _previousKeyboardState;        
        public static MouseForm MouseState = new MouseForm();

        public static void Init()
        {
            // Initialize 2D array
            _previousKeyboardState = Keyboard.GetState();

        }
        public static void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                Key[(int)key] = _currentKeyboardState.IsKeyDown(key);
                if (_previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key))
                {
                    // key up
                    Console.WriteLine($"Key: {key}, Value: {(int)key} UP");

                }
                if (_previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key))
                {
                    // key down
                    Player.OnKeyPress(key);
                    Console.WriteLine($"Key: {key}, Value: {(int)key} DOWN");

                }

            }

        }
    }
    public class MouseForm
    {
        
    }
}
