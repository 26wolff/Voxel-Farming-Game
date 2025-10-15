using Microsoft.Xna.Framework;

namespace Program
{
    public class UpdateManager
    {

        public void Update(GameTime gameTime, InputManager input)
        {
            Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds, input);
            
        }
    }
}
