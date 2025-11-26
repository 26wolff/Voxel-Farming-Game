using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using ComputeSharp;

namespace Program
{

    public class GameCore : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;
        public int TargetFramesPerSecond = 180;
        public float FPS = 0;
        public int times = 0;
        public float totalTime = 0f;

        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / TargetFramesPerSecond);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize your game systems
            //GameDataSync.Reset(true);
            
            GameDataSync.Sync();
            World.Init();
            Player.Init();
            UpdateManager.Init();
            DrawManager.Init(GraphicsDevice, _spriteBatch);
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
            Console.WriteLine($"Resolution updated: {Screen.Width}x{Screen.Height}, Aspect: {Screen.Aspect}");
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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            times++;
            FPS += dt;
            totalTime += dt;
            if (totalTime >= 1)
            {
                Console.WriteLine($"FPS: {1 / (FPS / times)}");
                totalTime = 0f;
                times = 0;
                FPS = 0f;
            }

            InputManager.Update(this);
            UpdateManager.Update(dt);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawManager.Draw(false);
            base.Draw(gameTime);
        }

        public static void Main()
        {
            using var game = new GameCore();
            game.Run();
        }
    }
}
