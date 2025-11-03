using Microsoft.Xna.Framework;

namespace Program
{
    public static class UpdateManager
    {
        public static void Init()
        {
            
        }
        
        public static void Update(GameTime gameTime)
        {
            InputManager.Update();
            Player.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            
        }
    }
}
