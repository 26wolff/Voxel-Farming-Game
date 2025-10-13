using Microsoft.Xna.Framework;

namespace Program
{
    public class UpdateManager
    {
        // Make Position a public field
        public Vector2 Position = Vector2.Zero;

        public void Update(GameTime gameTime, InputManager input)
        {
            if (input.Right) Position.X += 3;
            if (input.Left) Position.X -= 3;
            if (input.Up) Position.Y -= 3;
            if (input.Down) Position.Y += 3;
        }
    }
}
