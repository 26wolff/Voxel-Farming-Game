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

            // Construct path in AppData\Local\VoxelFarm\Data\World\Test.png
            string imagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "VoxelFarm",
                "Data",
                "Resources",
                "Test.png"
            );

            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"Image not found: {imagePath}");
                return;
            }

            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                _myTexture = Texture2D.FromStream(graphics, fileStream);
            }

            Console.WriteLine($"Loaded image from: {imagePath}");
        }

        public void Draw(GameTime gameTime, UpdateManager update)
        {
            if (_spriteBatch == null || _squareTexture == null || _myTexture == null)
                return;

            _spriteBatch.Begin();

            // Draw the loaded image at the player's position
            _spriteBatch.Draw(
                texture: _myTexture,
                position: new Vector2((int)(Player.Position.X * Screen.Resolution), (int)(Player.Position.Y * Screen.Resolution)),
                sourceRectangle: null,
                color: Color.White,
                rotation: 4f,
                origin: Vector2.Zero,
                scale: 0.15f * Screen.Resolution / _myTexture.Height,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
            
            

            _spriteBatch.End();
        }
    }
}
