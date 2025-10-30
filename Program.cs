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

            GameDataSync.Reset(true);
            GameDataSync.Sync();

            Player.Init();
            World.Init();

            SetWindowedMode();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnWindowResize;
        }

        private void OnWindowResize(object? sender, EventArgs e)
        {
            Screen.Resolution = Window.ClientBounds.Height;
            Screen.Aspect = Window.ClientBounds.Width / Screen.Resolution;
            Console.WriteLine($"Resolution updated: {Screen.Resolution}, Aspect updated: {Screen.Aspect}");
        }
        private void SetWindowedMode(int Resolution = 500)
        {
            Screen.Resolution = Resolution;
            _graphics.PreferredBackBufferWidth = (int)(Resolution * Screen.Aspect);
            _graphics.PreferredBackBufferHeight = Resolution;

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

            if (_inputManager.Exit) Exit();

            if (_inputManager.F11) Player.Save();

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
