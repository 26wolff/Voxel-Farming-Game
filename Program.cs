using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Program
{
    public class GameCore : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Managers
        private UpdateManager _updateManager;
        private DrawManager _drawManager;
        private InputManager _inputManager;

        public float ScreenAspect = 16f/9f;
        public int ScreenResolution = 0;

        private bool _isFullscreen = false;

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



        private void OnWindowResize(object sender, EventArgs e)
        {
            var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            ScreenResolution = Window.ClientBounds.Height;;
            Console.WriteLine(ScreenResolution);

        }
        private void SetFullscreenMode()
        {
            var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            ScreenResolution = display.Height;

            _graphics.PreferredBackBufferWidth = (int)(ScreenResolution * ScreenAspect);
            _graphics.PreferredBackBufferHeight = ScreenResolution;

            _graphics.IsFullScreen = false; // Borderless fullscreen windowed
            _graphics.ApplyChanges();


            Window.IsBorderless = true;
            Window.Position = new Point(0, 0);
        }

        private void SetWindowedMode()
        {
            int windowHeight = 500;
            int windowWidth = (int)(windowHeight * ScreenAspect);

            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;

            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Window.IsBorderless = false;
            Window.Position = new Point(100, 100); // Or center as needed
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _drawManager.LoadContent(GraphicsDevice, _spriteBatch);

            int width = _graphics.PreferredBackBufferWidth;
            int height = _graphics.PreferredBackBufferHeight;

            Console.WriteLine($"Current screen size: {width}x{height}");
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
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
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
