using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Program
{
    public class DrawManager
    {
        private SpriteBatch? _spriteBatch;
        private Texture2D? _squareTexture;
        private Texture2D? _myTexture;

        public void LoadContent(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;

            // Create a white square texture for testing
            _squareTexture = new Texture2D(graphics, 1, 1);
            _squareTexture.SetData(new[] { Color.White });

            // Construct path relative to executable
            string imagePath = Path.Combine(AppContext.BaseDirectory, "Data", "World", "Test.png");

            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open))
            {
                _myTexture = Texture2D.FromStream(graphics, fileStream);
            }
        }

        public void Draw(GameTime gameTime, UpdateManager update)
        {
            if (_spriteBatch == null || _squareTexture == null || _myTexture == null)
                return;

            _spriteBatch.Begin();

            // Draw the loaded image at the player's position
            
            _spriteBatch.Draw(
                texture: _myTexture,          // your Texture2D
                position: new Vector2((int)(Player.Position.X * Screen.Resolution), (int)(Player.Position.Y * Screen.Resolution)), // top-left position
                sourceRectangle: null,       // null means use full texture
                color: Color.White,          // color tint (White = no change)
                rotation: 0f,                // rotation in radians
                origin: Vector2.Zero,        // rotation/scaling origin
                scale: 0.15f * Screen.Resolution / _myTexture.Height,                   // scale factor (2 = double size)
                effects: SpriteEffects.None, // flip effects
                layerDepth: 0f               // draw order
            );

            _spriteBatch.End();
        }
    }
}
