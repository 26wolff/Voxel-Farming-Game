using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Program
{
    public class DrawManager
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _squareTexture;

        public void LoadContent(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _squareTexture = new Texture2D(graphics, 1, 1);
            _squareTexture.SetData(new[] { Color.White });
        }

        public void Draw(GameTime gameTime, UpdateManager update)
        {
            _spriteBatch.Begin();
            _spriteBatch.Draw(
                _squareTexture,
                new Rectangle((int)update.Position.X, (int)update.Position.Y, 50, 50),
                Color.Red
            );
            _spriteBatch.End();
        }
    }
}
