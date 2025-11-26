using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Program
{
    public static class InputManager
    {
        public static bool[] Key = new bool[255];
        private static KeyboardState _currentKeyboardState;
        private static KeyboardState _previousKeyboardState;

        private static MouseState _currentMouseState;
        private static MouseState _previousMouseState;

        private static bool _mouseLocked = false;
        private static Point _centerScreen;
        private static float _mouseSensitivity = 0.003f; // tweak

        public static MouseForm MouseState = new MouseForm();

        public static void Init()
        {
            _previousKeyboardState = Keyboard.GetState();
            _previousMouseState = Mouse.GetState();

            UpdateScreenCenter();
        }

        public static void Update(GameCore game)
        {
            // --- keyboard ---
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            // --- mouse ---
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            UpdateScreenCenter();

            // --- lock/unlock mouse ---
            if (!_mouseLocked && _currentMouseState.LeftButton == ButtonState.Pressed && game.IsActive)
            {
                _mouseLocked = true;
                game.IsMouseVisible = false;
                Mouse.SetPosition(_centerScreen.X, _centerScreen.Y);
            }
            else if (_mouseLocked && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _mouseLocked = false;
                game.IsMouseVisible = true;
            }

            // --- apply movement when locked ---
            if (_mouseLocked && game.IsActive)
            {
                int deltaX = _centerScreen.X - _currentMouseState.X; // invert LR
                int deltaY = _currentMouseState.Y - _centerScreen.Y; // natural up/down

                if (deltaX != 0 || deltaY != 0)
                {
                    MouseState.Delta = new Point(deltaX, deltaY);

                    // Update yaw (horizontal) with wrapping
                    Camera.ViewAngle.X += deltaX * _mouseSensitivity;
                    Camera.ViewAngle.X = WrapAngle(Camera.ViewAngle.X);

                    // Update pitch (vertical) with clamp
                    Camera.ViewAngle.Y += -deltaY * _mouseSensitivity;
                    Camera.ViewAngle.Y = MathHelper.Clamp(Camera.ViewAngle.Y, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);
                    // Reset mouse to center
                    Mouse.SetPosition(_centerScreen.X, _centerScreen.Y);
                }
            }

            // --- keyboard handling ---
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                Key[(int)key] = _currentKeyboardState.IsKeyDown(key);

                if (_previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyDown(key))
                    Player.On_Key_Press(key);
                    Camera.On_Key_Press(key);
                if (_previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key))
                    Player.On_Key_Release(key);
                    Camera.On_Key_Release(key);
            }
        }

        private static void UpdateScreenCenter()
        {
            _centerScreen = new Point(
                (int)(Screen.Width / 2f),
                (int)(Screen.Height / 2f)
            );
        }

        private static float WrapAngle(float angle)
        {
            while (angle >= MathHelper.TwoPi) angle -= MathHelper.TwoPi;
            while (angle < 0) angle += MathHelper.TwoPi;
            return angle;
        }
    }

    public class MouseForm
    {
        public Point Delta = Point.Zero;
    }
}
