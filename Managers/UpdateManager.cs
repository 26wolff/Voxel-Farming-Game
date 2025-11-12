using Microsoft.Xna.Framework;

namespace Program
{
    public static class UpdateManager
    {
        public static void Init()
        {
            
        }
        
        public static void Update(float dt)
        {
            Player.Update(dt);
            Camera.Update(dt);
            
        }
    }
}
