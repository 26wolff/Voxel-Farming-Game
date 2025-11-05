using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Program
{
    public static class DrawManager
    {
        private static SpriteBatch? _spriteBatch;
        private static GraphicsDevice? _graphicsDevice;
        private static bool rep = true;

        public static void Init(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
        }

        public static void Draw()
        {
            int[][] ToRender = World.GetChunksToRender();
            if (rep)
            {
                Console.WriteLine("DREW");
                foreach(var c in ToRender)
                {
                    Console.WriteLine($"{c[0]},{c[2]}");
                }
                Console.WriteLine();



                rep = false;
            }

            // Example: draw a red square
            _spriteBatch!.Begin();
            var tex = CreateTexture(Color.Red);
            _spriteBatch.Draw(tex, new Rectangle((int)(Player.Position.X*Screen.Width), (int)(-Player.Position.Z*Screen.Height), 64, 64), Color.White);
            _spriteBatch.End();
        }

        public static Texture2D CreateTexture(Color color)
        {
            // if graphics device is still null, try to grab it from the spritebatch
            if (_graphicsDevice == null && _spriteBatch != null)
                _graphicsDevice = _spriteBatch.GraphicsDevice;

            if (_graphicsDevice == null)
                throw new Exception("DrawManager.Init() must be called before creating textures.");

            Texture2D texture = new Texture2D(_graphicsDevice, 1, 1);
            texture.SetData(new[] { color });
            return texture;
        }
    }
}