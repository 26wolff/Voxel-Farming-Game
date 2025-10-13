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

        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _updateManager = new UpdateManager();
            _drawManager = new DrawManager();
            _inputManager = new InputManager();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _drawManager.LoadContent(GraphicsDevice, _spriteBatch);
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
