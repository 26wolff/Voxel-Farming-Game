using System;
using System.IO;
using System.Text.Json;
using System.Numerics;

namespace Program
{
    public static class Camera
    {
        public static Vector3 Position = Player.Position;
        public static Vector2 ViewAngle = Vector2.Zero;
        public static float Fov = (float) Math.PI;

        public static void Update()
        {
            
        }
    }
}