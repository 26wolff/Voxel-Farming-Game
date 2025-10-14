using Microsoft.Xna.Framework;

namespace Program
{
    public class UpdateManager
    {

        public void Update(GameTime gameTime, InputManager input)
        {
            if (input.Right) Player.Position.X += 0.03f;
            if (input.Left) Player.Position.X -= 0.03f;
            if (input.Up) Player.Position.Y -= 0.03f;
            if (input.Down) Player.Position.Y += 0.03f;
        }
    }
}
