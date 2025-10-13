using Microsoft.Xna.Framework.Input;

namespace Program
{
    public class InputManager
    {
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Left { get; private set; }
        public bool Right { get; private set; }
        public bool Exit { get; private set; }

        public void HandleInput()
        {
            var keyboard = Keyboard.GetState();

            Up = keyboard.IsKeyDown(Keys.Up);
            Down = keyboard.IsKeyDown(Keys.Down);
            Left = keyboard.IsKeyDown(Keys.Left);
            Right = keyboard.IsKeyDown(Keys.Right);
            Exit = keyboard.IsKeyDown(Keys.Escape);
        }
    }
}
