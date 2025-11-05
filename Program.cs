using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;

namespace Program
{
    public static class Screen
    {
        public static float Aspect { get; set; } = 16f / 9f;
        public static float Width { get; set; } = 0;
        public static float Height { get; set; } = 0;

    }

    public class GameCore : Game
    {
        private GraphicsDeviceManager _graphics;

        private SpriteBatch? _spriteBatch;

        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //GameDataSync.Reset(true);
            GameDataSync.Sync();

            World.Init();
            Player.Init();

            UpdateManager.Init();
            DrawManager.Init(GraphicsDevice,_spriteBatch);
            InputManager.Init();

            SetWindowedMode();
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnWindowResize;
        }

        private void OnWindowResize(object? sender, EventArgs e)
        {
            Screen.Height = Window.ClientBounds.Height;
            Screen.Width = Window.ClientBounds.Width;
            Screen.Aspect = Screen.Width / Screen.Height;
            Console.WriteLine($"Resolution updated: {Screen.Height} by {Screen.Width}, Aspect updated: {Screen.Aspect}");
        }

        private void SetWindowedMode(int Resolution = 500)
        {
            Screen.Height = Resolution;
            Screen.Width = Resolution * Screen.Aspect;
            _graphics.PreferredBackBufferWidth = (int)Screen.Width;
            _graphics.PreferredBackBufferHeight = Resolution;

            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Window.IsBorderless = false;
            Window.Position = new Point(100, 100);
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            DrawManager.Draw();
            base.Draw(gameTime);
        }

        public static void Main()
        {
            using var game = new GameCore();
            game.Run();
        }
    }
}
