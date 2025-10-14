using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Program
{
    public static class Screen
    {
        public static float Aspect { get; set; } = 16f / 9f;
        public static float Resolution { get; set; } = 0;
    }

    public class GameCore : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;

        // Managers
        private UpdateManager _updateManager;
        private DrawManager _drawManager;
        private InputManager _inputManager;

        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _updateManager = new UpdateManager();
            _drawManager = new DrawManager();
            _inputManager = new InputManager();

            SetWindowedMode();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnWindowResize;
        }

        private void OnWindowResize(object? sender, EventArgs e)
        {
            Screen.Resolution = Window.ClientBounds.Height;
            Console.WriteLine($"Resolution updated: {Screen.Resolution}");
        }

        private void SetFullscreenMode()
        {
            var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            Screen.Resolution = display.Height;

            _graphics.PreferredBackBufferWidth = (int)(Screen.Resolution * Screen.Aspect);
            _graphics.PreferredBackBufferHeight = (int)Screen.Resolution;

            _graphics.IsFullScreen = false; // Borderless fullscreen windowed
            _graphics.ApplyChanges();

            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);
        }

        private void SetWindowedMode()
        {
            int windowHeight = 500;
            int windowWidth = (int)(windowHeight * Screen.Aspect);

            Screen.Resolution = windowHeight;

            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;

            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Window.IsBorderless = false;
            Window.Position = new Point(100, 100);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _drawManager.LoadContent(GraphicsDevice, _spriteBatch);//, Content); // <-- Pass Content

            Console.WriteLine($"Current screen size: {_graphics.PreferredBackBufferWidth}x{_graphics.PreferredBackBufferHeight}");
        }

        protected override void Update(GameTime gameTime)
        {
            _inputManager.HandleInput();

            if (_inputManager.Exit)
                Exit();

            _updateManager.Update(gameTime, _inputManager);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _drawManager.Draw(gameTime, _updateManager);
            base.Draw(gameTime);
        }

        public static void Main()
        {
            using var game = new GameCore();
            game.Run();
        }
    }
}
