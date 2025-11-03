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


        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            

            //GameDataSync.Reset(true);
            GameDataSync.Sync();

            UpdateManager.Init();
            DrawManager.Init();
            InputManager.Init();
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

        protected override void Update(GameTime gameTime)
        {
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
