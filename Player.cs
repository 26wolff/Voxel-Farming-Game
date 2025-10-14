using System.IO;
using System.Text.Json;
using System.Numerics;

namespace Program
{
    public static class Player
    {
        public static Vector3 Position = Vector3.Zero;
        public static Vector2 VeiwAngle = Vector2.Zero;

        public static void Innit()
        {
            var path = "../../../Data/Player/PlayerData.json";
            string jsonString = File.ReadAllText(path);
            Console.WriteLine(jsonString);
        }
    }
}